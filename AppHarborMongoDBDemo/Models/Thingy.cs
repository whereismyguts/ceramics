
using System.Collections.Generic;

namespace AppHarborMongoDBDemo
{
	public class Thingy : Entity
	{
		public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; } = string.Empty;
        public int Price { get; set; }
        public List<byte[]> Images { get; internal set; }
    }
}
