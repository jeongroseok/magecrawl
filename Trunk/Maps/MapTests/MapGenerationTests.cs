using NUnit.Framework;
using Magecrawl.Maps.Generator.Cave;
using libtcod;
using Magecrawl.Maps.Generator.Stitch;

namespace Magecrawl.Maps.Tests
{
    [TestFixture]
    public class MapGenerationTests
    {
        private const int NumberOfMapsToGenerate = 500;
        private const int MaxLevelOfLevelToGenerate = 5;

        [Test]
        public void GenerateLotsOfCaveMaps()
        {
            TCODRandom random = new TCODRandom();
            for (int i = 0; i < NumberOfMapsToGenerate; i++)
            {
                SimpleCaveGenerator generator = new SimpleCaveGenerator(random);
                Map map = generator.GenerateMap(null, random.getInt(0, MaxLevelOfLevelToGenerate));
                Assert.IsNotNull(map);
            }
        }


        [Test]
        public void GenerateLotsOfStitchMaps()
        {
            TCODRandom random = new TCODRandom();
            for (int i = 0; i < NumberOfMapsToGenerate; i++)
            {
                StitchtogeatherMapGenerator generator = new StitchtogeatherMapGenerator(random); 
                Map map = generator.GenerateMap(null, random.getInt(0, MaxLevelOfLevelToGenerate));
                Assert.IsNotNull(map);
            }
        }
    }
}
