using System.Collections.Generic;

namespace AppHarborMongoDBDemo {
    public class UserCart : Entity {

        public UserCart(string userId, string itemId) {
            UserId = userId;
            Items.Add( itemId);
        }

        public string UserId { get; set; } = "";
        public List<string> Items { get; set; } = new List<string>();
    }
}