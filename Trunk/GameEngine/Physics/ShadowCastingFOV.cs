using System;

namespace Magecrawl.GameEngine.Physics
{
    // This code is taken almost directly from fov_recursive_shadowcasting.c
    // in libtcod, with formatting changes and a few interface changes
    // It is excempt from most any code formatting rules, since I try to
    // keep the internals as close to C as possible to make porting changes easy
    internal static class ShadowCastingFOV
    {
        private static int [,] mult = new int[4,8] {{1,0,0,-1,-1,0,0,1},
                                                    {0,1,-1,0,0,-1,1,0},
                                                    {0,1,1,0,0,-1,-1,0},
                                                    {1,0,0,1,-1,0,0,-1}};

        private static void CastLight(PhysicsMap map, int cx, int cy, int row, float start, float end, int radius, int r2, int xx, int xy, int yx, int yy, int id, bool light_walls) 
        {
            float new_start = 0.0f;
            if (start < end)
                return;
        
            for (int j = row; j < radius + 1; j++) 
            {
                int dx = -j - 1;
                int dy = - j;
                bool blocked = false;
                while (dx <= 0)
                {
                    dx++;
                    int X = cx + dx * xx + dy * xy;
                    int Y = cy + dx * yx + dy * yy;
                    if ((uint)X < (uint)map.Width && (uint)Y < (uint)map.Height) 
                    {
                        int offset = X + Y * map.Width;
                        float l_slope = (dx - 0.5f) / (dy + 0.5f);
                        float r_slope = (dx + 0.5f) / (dy - 0.5f);
                        if (start < r_slope)
                            continue;
                        else if(end > l_slope)
                            break;
                        if (dx * dx + dy * dy <= r2 && (light_walls || map.Cells[X, Y].Transparent))
                            map.Cells[X, Y].Visible = true;
                        if ( blocked ) 
                        {
                            if (!map.Cells[X, Y].Transparent)
                            {
                                new_start = r_slope;
                                continue;
                            }
                            else 
                            {
                                blocked = false;
                                start = new_start;
                            }
                        } 
                        else 
                        {
                            if (!map.Cells[X, Y].Transparent && j < radius)
                            {
                                blocked = true;
                                CastLight(map, cx, cy, j + 1, start, l_slope, radius, r2, xx, xy, yx, yy, id+1, light_walls);
                                new_start = r_slope;
                            }
                        }
                    }
                }
                if (blocked)
                    break;
            }
        }

        internal static void ComputeRecursiveShadowcasting(PhysicsMap map, int playerX, int playerY, int maxRadius, bool lightWalls)
        {
            map.ClearVisibility();

            if ( maxRadius == 0 ) 
            {
                int max_radius_x = map.Width - playerX;
                int max_radius_y = map.Height - playerY;
                max_radius_x = Math.Max(max_radius_x, playerX);
                max_radius_y = Math.Max(max_radius_y, playerY);
                maxRadius = (int)(Math.Sqrt(max_radius_x * max_radius_x + max_radius_y * max_radius_y)) + 1;
            }
            int r2 = maxRadius * maxRadius;
            /* recursive shadow casting */
            for (int oct = 0; oct < 8; oct++)
            {
                CastLight(map, playerX, playerY, 1, 1.0f, 0.0f, maxRadius, r2, mult[0,oct], mult[1,oct], mult[2,oct], mult[3,oct], 0, lightWalls);
            }
            map.Cells[playerX, playerY].Visible = true;
        }
    }
}
