using System;
using System.Collections.Generic;
using System.Linq;

namespace AppHarborMongoDBDemo {
    public class CartItem {
        public CartItem(Thingy thing) {
            Thing = thing;
        }
        public Thingy Thing { get; set; }
        public int Count { get; set; } = 1;
        public int Price { get { return Thing.Price * Count; } }

        public override string ToString() {
            return Thing.ToString() + " - " + Count + " шт. ("+Price+" р)";
        }
    }

    public class CartItems {

        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int Count { get { return Items.Sum(i => i.Count); } }
        public int Price { get { return Items.Sum(i => i.Price); } }


        internal void Add(Thingy thing) {
            var same = Items.FirstOrDefault(i => i.Thing.Id == thing.Id);
            if(same != null) {
                same.Count++;
            }
            else
                Items.Add(new CartItem(thing));


           // Items.Sort();
        }

        public override string ToString() {
            string res = "";
            foreach(var item in Items)
                res += item.ToString() + "\n";
            return res;
        }
    }
}