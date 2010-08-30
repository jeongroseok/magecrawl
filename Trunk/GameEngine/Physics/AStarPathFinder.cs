using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Physics
{
    // This code is taken almost directly from path_c.c
    // in libtcod, with formatting changes and a few interface changes
    // It is excempt from most any code formatting rules, since I try to
    // keep the internals as close to C as possible to make porting changes easy
    internal class AStarPathFinder
    {
        private enum PathFindingDirection { NORTH_WEST, NORTH, NORTH_EAST, WEST, NONE, EAST, SOUTH_WEST, SOUTH, SOUTH_EAST };

        /* convert dir_t to dx,dy */
        private static int[] dirx = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        private static int [] diry = {-1,-1,-1,0,0,0,1,1,1};

        private int m_ox, m_oy; /* coordinates of the creature position */
        private int m_dx, m_dy; /* coordinates of the creature's destination */

        private List<PathFindingDirection> m_path;

        private int m_width, m_height; /* map size */

        private float[] m_grid; /* wxh djikstra distance grid (covered distance) */
        private float[] m_heur;
        private PathFindingDirection[] m_prev;

        private float m_diagonalCost;

        private List<uint> m_heap;

        private PhysicsMap m_map;

        #region Heap Functions
        // libtcod's version uses a list that these functions turn into a heap
        private void heap_sift_down()
        {
            /* sift-down : move the first element of the heap down to its right place */
            int cur = 0;
            int end = m_heap.Count - 1;
            int child = 1;

            while (child <= end)
            {
                int toSwap = cur;
                uint off_cur = m_heap[cur];
                float cur_dist = m_heur[off_cur];
                float swapValue = cur_dist;
                uint off_child = m_heap[child];
                float child_dist = m_heur[off_child];
                if (child_dist < cur_dist)
                {
                    toSwap = child;
                    swapValue = child_dist;
                }
                if (child < end)
                {
                    /* get the min between child and child+1 */
                    uint off_child2 = m_heap[child + 1];
                    float child2_dist = m_heur[off_child2];
                    if (swapValue > child2_dist)
                    {
                        toSwap = child + 1;
                        swapValue = child2_dist;
                    }
                }
                if (toSwap != cur)
                {
                    /* get down one level */
                    uint tmp = m_heap[toSwap];
                    m_heap[toSwap] = m_heap[cur];
                    m_heap[cur] = tmp;
                    cur = toSwap;
                }
                else return;
                child = cur * 2 + 1;
            }
        }

        private void heap_sift_up()
        {
            /* sift-up : move the last element of the heap up to its right place */
            int end = m_heap.Count - 1;
            int child = end;
            while (child > 0)
            {
                uint off_child = m_heap[child];
                float child_dist = m_heur[off_child];
                int parent = (child - 1) / 2;
                uint off_parent = m_heap[parent];
                float parent_dist = m_heur[off_parent];
                if (parent_dist > child_dist)
                {
                    /* get up one level */
                    uint tmp = m_heap[child];
                    m_heap[child] = m_heap[parent];
                    m_heap[parent] = tmp;
                    child = parent;
                }
                else
                {
                    return;
                }
            }
        }
        
        /* add a coordinate pair in the heap so that the heap root always contains the minimum A* score */
        private void heap_add(int x, int y)
        {
            /* append the new value to the end of the heap */
            uint off = (uint)(x + y * m_width);
            m_heap.Add(off);
            /* bubble the value up to its real position */
            heap_sift_up();
        }

        /* get the coordinate pair with the minimum A* score from the heap */
        private uint heap_get()
        {
            /* return the first value of the heap (minimum score) */
            int end = m_heap.Count() - 1;
            uint off = m_heap[0];
            /* take the last element and put it at first position (heap root) */
            m_heap[0] = m_heap[end];
            m_heap.RemoveAt(end);
            /* and bubble it down to its real position */
            heap_sift_down();
            return off;
        }

        /* this is the slow part, when we change the heuristic of a cell already in the heap */
        private void heap_reorder(uint offset)
        {
            uint off_idx = 0;
            float value;
            int idx = -1;
            int heap_size = m_heap.Count();

            for (int i = 0; i < heap_size; ++i)
            {
                if (m_heap[i] == offset)
                {
                    idx = i;
                    break;
                }
            }

            if (idx == -1)
                return;

            off_idx = m_heap[idx];
            value = m_heur[off_idx];
            if (idx > 0)
            {
                int parent = (idx - 1) / 2;
                /* compare to its parent */
                uint off_parent = m_heap[parent];
                float parent_value = m_heur[off_parent];
                if (value < parent_value)
                {
                    /* smaller. bubble it up */
                    while (idx > 0 && value < parent_value)
                    {
                        /* swap with parent */
                        m_heap[parent] = off_idx;
                        m_heap[idx] = off_parent;
                        idx = parent;
                        if (idx > 0)
                        {
                            parent = (idx - 1) / 2;
                            off_parent = m_heap[parent];
                            parent_value = m_heur[off_parent];
                        }
                    }
                    return;
                }
            }
            /* compare to its sons */
            while (idx * 2 + 1 < heap_size)
            {
                int child = idx * 2 + 1;
                uint off_child = m_heap[child];
                int toSwap = idx;
                int child2;
                float swapValue = value;
                if (m_heur[off_child] < value)
                {
                    /* swap with son1 ? */
                    toSwap = child;
                    swapValue = m_heur[off_child];
                }
                child2 = child + 1;
                if (child2 < heap_size)
                {
                    uint off_child2 = m_heap[child2];
                    if (m_heur[off_child2] < swapValue)
                    {
                        /* swap with son2 */
                        toSwap = child2;
                    }
                }
                if (toSwap != idx)
                {
                    /* bigger. bubble it down */
                    uint tmp = m_heap[toSwap];
                    m_heap[toSwap] = m_heap[idx];
                    m_heap[idx] = tmp;
                    idx = toSwap;
                }
                else return;
            }
        }
        #endregion

        internal AStarPathFinder(PhysicsMap map, float diagonalCost) 
        {        	
            m_width = map.Width;
            m_height = map.Height;

            m_grid = new float[m_width + m_height * m_width];
            m_heur = new float[m_width + m_height * m_width];
            m_prev = new PathFindingDirection[m_width + m_height * m_width];
            m_path = new List<PathFindingDirection>();
            m_heap = new List<uint>();

	        m_map = map;
	        m_diagonalCost = diagonalCost;
        }

        public bool Compute(int ox, int oy, int dx, int dy) 
        {
	        m_ox = ox;
	        m_oy = oy;
	        m_dx = dx;
	        m_dy = dy;
            m_path.Clear();
            m_heap.Clear();

            if ( ox == dx && oy == dy ) 
                return true; /* trivial case */

            /* check that origin and destination are inside the map */
            if (!((uint)ox < (uint)m_width && (uint)oy < (uint)m_height))
                return false;
            if (!((uint)dx < (uint)m_width && (uint)dy < (uint)m_height))
                return false;

            Array.Clear(m_grid, 0, m_width * m_height);
            Array.Clear(m_prev, 0, m_width * m_height);
            m_heur[ox + oy * m_width] = 1.0f;

	        TCOD_path_push_cell(ox, oy); /* put the origin cell as a bootstrap */
	        /* fill the djikstra grid until we reach dx,dy */
	        TCOD_path_set_cells();

	        if ( m_grid[dx + dy * m_width] == 0)
                return false; /* no path found */

	        /* there is a path. retrieve it */
	        do 
            {
		        /* walk from destination to origin, using the 'prev' array */
                PathFindingDirection step = m_prev[dx + dy * m_width];
                m_path.Add(step);
                dx -= dirx[(int)step];
                dy -= diry[(int)step];
	        } 
            while ( dx != ox || dy != oy );
	        return true;
        }

        public bool Walk(ref int x, ref int y, bool recalculate_when_needed) 
        {
            if (m_path.Count == 0)
                return false;
            PathFindingDirection d = m_path[0];
            m_path.RemoveAt(0);
	        
	        int newx = m_ox + dirx[(int)d];
            int newy = m_oy + diry[(int)d];

	        /* check if the path is still valid */
	        if ( m_map.Cells[newx, newy].Walkable ) 
            {
		        if (! recalculate_when_needed )
                    return false; /* don't walk */
		        /* calculate a new path */
                if (!Compute(m_ox, m_oy, m_dx, m_dy))
                    return false ; /* cannot find a new path */

                return Walk(ref x, ref y, true); /* walk along the new path */
	        }

	        x = newx;
	        y = newy;
	        m_ox = newx;
            m_oy = newy;
	        return true;
        }

        public bool IsEmpty()
        {
            return m_path.Count == 0;
        }

        public int Size()
        {
            return m_path.Count;
        }

        public void GetPathElement(int index, out int x, out int y)
        {
            x = m_ox;
            y = m_oy;
            int pos = m_path.Count - 1;
	        do 
            {
                PathFindingDirection step = m_path[pos];
                x += dirx[(int)step];
                y += diry[(int)step];
		        pos--;
                index--;
	        } 
            while (index >= 0);
        }

        /* private stuff */
        /* add a new unvisited cells to the cells-to-treat list
         * the list is in fact a min_heap. Cell at index i has its sons at 2*i+1 and 2*i+2
         */
        private void TCOD_path_push_cell(int x, int y)
        {
            heap_add(x, y);
        }

        /* get the best cell from the heap */
        private void TCOD_path_get_cell(out int x, out int y, out float distance)
        {
            uint offset = heap_get();
            x = (int)(offset % m_width);
            y = (int)(offset / m_width);
            distance = m_grid[offset];
        }

        private float TCOD_path_walk_cost(int xFrom, int yFrom, int xTo, int yTo)
        {
            return m_map.Cells[xTo, yTo].Walkable ? 1.0f : 0.0f;         
        }

        private void TCOD_path_get_origin(out int x, out int y) 
        {
            x = m_ox;
            y = m_oy;
        }

        private void TCOD_path_get_destination(out int x, out int y)
        {
            x = m_dx;
            y = m_dy;
        }

        int [] idirx = new int[] {0,-1,1,0,-1,1,-1,1};
        int [] idiry = new int[] {-1,0,0,1,-1,-1,1,1};
        PathFindingDirection[] prevdirs = { PathFindingDirection.NORTH, PathFindingDirection.WEST, PathFindingDirection.EAST, PathFindingDirection.SOUTH, 
                                              PathFindingDirection.NORTH_WEST, PathFindingDirection.NORTH_EAST, PathFindingDirection.SOUTH_WEST, 
                                              PathFindingDirection.SOUTH_EAST };

        /* fill the grid, starting from the origin until we reach the destination */
        private void TCOD_path_set_cells() 
        {
	        while (m_grid[m_dx + m_dy * m_width ] == 0 && !(m_heap.Count == 0)) 
            {
                int x,y;
		        float distance;
		        TCOD_path_get_cell(out x, out y,out distance);

		        int imax = ( m_diagonalCost == 0.0f ? 4 : 8) ;
		        for (int i = 0; i < imax; i++ ) 
                {
			        /* convert i to dx,dy */
			        /* convert i to direction */
			        /* coordinate of the adjacent cell */
			        int cx = x + idirx[i];
			        int cy = y + idiry[i];
			        if ( cx >= 0 && cy >= 0 && cx < m_width && cy < m_height )
                    {
                        float walk_cost = TCOD_path_walk_cost(x, y, cx, cy);
				        if (walk_cost > 0.0f)
                        {
					        /* in of the map and walkable */
					        float covered=distance + walk_cost * (i>=4 ? m_diagonalCost : 1.0f);
					        float previousCovered = m_grid[cx + cy * m_width ];
					        if ( previousCovered == 0 ) 
                            {
						        /* put a new cell in the heap */
						        int offset = cx + cy * m_width;
						        /* A* heuristic : remaining distance */
						        float remaining=(float)Math.Sqrt((cx - m_dx)*(cx - m_dx)+(cy - m_dy)*(cy - m_dy));
						        m_grid[offset] = covered;
                                m_heur[offset] = covered + remaining;
                                m_prev[offset] = prevdirs[i];
						        TCOD_path_push_cell(cx,cy);
					        } 
                            else if ( previousCovered > covered ) 
                            {
						        /* we found a better path to a cell already in the heap */
						        int offset = cx + cy * m_width;
						        m_grid[ offset ] = covered;
						        m_heur[ offset ] -= (previousCovered - covered); /* fix the A* score */
						        m_prev[ offset ] =  prevdirs[i];
						        /* reorder the heap */
						        heap_reorder((uint)offset);
					        }
				        }
			        }
		        }
	        }
        }
    }
}
