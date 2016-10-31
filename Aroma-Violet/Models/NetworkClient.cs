using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class NetworkClient
    {
        public List<NetworkClient> Children { get; private set; }
        public int ClientId { get; private set; }
        public int DescendantCount { get; private set; }
        public int DirectChildCount { get; private set; }
        public int? Parent { get; set; }
        internal void LoadNetwork(AromaContext db)
        {
            this.Parent = db.Clients.Find(this.ClientId).ResellerID;
            var children = (from item in db.Clients
                            where item.ResellerID == this.ClientId
                            select item.ClientId).ToArray();
            this.DirectChildCount = children.Length;
            this.DescendantCount = this.DirectChildCount;
            this.Children = new List<NetworkClient>();
            foreach (var id in children)
            {
                var child = new NetworkClient();
                child.LoadNetwork(db, id);
                this.Children.Add(child);
                this.DescendantCount += child.DescendantCount;
            }

        }
        internal void LoadNetwork(AromaContext db, int clientId)
        {
            if (clientId == 0) clientId = db.Clients.Where(m => m.ResellerID == null).First().ClientId;
            this.ClientId = clientId;
            LoadNetwork(db);
        }
    }
}