using System.Collections.Generic;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace CoffeeShop.Models
{
    public class HomeViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ImageViewModel> BannerImage { get; set; }
    }

    public class GlassHomeViewModel
    {
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        [SitecoreField("Show Banner")]
        public virtual bool IsShowBanner { get; set; }
        [SitecoreField("Banner")]
        public virtual IEnumerable<Image> BannerImage { get; set; }
    }
}