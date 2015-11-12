using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public class SMSManagerViewModel
    {
        [Display(Name = "Unsent SMS Count")]
        public int UnsentSMSCount { get; set; }

        [Display(Name = "Last SMS Added")]
        public DateTime? LastSMSAdded { get; set; }

        [Display(Name = "Last Send Attempt")]
        public DateTime? LastSendAttempt { get; set; }
        
        [Display(Name = "Last Successful Send")]
        public DateTime? LastSuccessfulSend { get; set; }
        [Display(Name = "SMS Template")]
        public int SystemSMSTemplateID { get; internal set; }
        public int ClientSMSCount { get; internal set; }
        public List<SMSDistributionItemModel> Countries { get; internal set; }
        public List<string> Variables { get; internal set; }
        public int ClientID { get; internal set; }
    }

    public class SMSDistributionItemModel
    {
        private Guid _key = Guid.NewGuid();
        public Guid Key {
            get
            {
                return _key;
            }
        }
        public int Id { get; set; }
        public string Description { get; set; }
        public SMSDistributionViewModel.enumSMSDistributionItemType ItemType { get; set; }
        private SMSDistributionItemModel _parent;
        public SMSDistributionItemModel Parent
        {
            get
            {
                if (this.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.Country) return null;

                if (_parent == null)
                {
                    int parentIndex = (int)this.ItemType - 1;
                    int parentId = (from item in SMSDistributionViewModel.RelationShips
                            where item[(int)this.ItemType] == this.Id
                            select item[parentIndex]).FirstOrDefault();
                    _parent = (from item in SMSDistributionViewModel.Items
                               where (int)item.ItemType == parentIndex
                               && item.Id == parentId
                               select item).FirstOrDefault();
                }
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }
        private List<SMSDistributionItemModel> _children = new List<SMSDistributionItemModel>();
        public List<SMSDistributionItemModel> Children
        {
            get
            {
                if (this.ItemType == SMSDistributionViewModel.enumSMSDistributionItemType.PostalCode) return _children;
                if (_children.Count() == 0 )
                {
                    int childIndex = (int)this.ItemType + 1;
                    int[] childrenIds = (from item in SMSDistributionViewModel.RelationShips
                                         where item[(int)this.ItemType] == this.Id
                                         select item[childIndex]).ToArray();
                    _children = (from item in SMSDistributionViewModel.Items
                                 where (int)item.ItemType == childIndex
                                 && childrenIds.Contains( item.Id) 
                                 select item).ToList();
                }
                return _children;
            }
            set
            {
                _children = value;
            }
        }
    }

    public static class SMSDistributionViewModel
    {
        
        public enum enumSMSDistributionItemType
        {
            Country=0,
            Province=1,
            PostalArea=2,
            PostalCode=3
        }

        public static List<SMSDistributionItemModel> Items { get; set; }

        public static int[][] RelationShips { get; set; }

        internal static IEnumerable<SMSDistributionItemModel> Find(int[] indexes, enumSMSDistributionItemType itemType)
        {
            var results = new List<SMSDistributionItemModel>();
            if (indexes != null)
            {
                results = (from item in Items
                           where (int)item.ItemType == (int)itemType
                           & indexes.Contains(item.Id)
                           select item).ToList();
            }
            return results;
        }
    }
}