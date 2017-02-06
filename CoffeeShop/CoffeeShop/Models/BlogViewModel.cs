using System.Collections.Generic;

namespace CoffeeShop.Models
{
    public class BlogViewModel
    {
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string CreatedDate { get; set; }
        public string LinkUrl { get; set; }
        public ImageViewModel BlogImage { get; set; }
        public List<PostViewModel> Posts { get; set; }
        public List<ImageViewModel> BannerImage { get; set; }
        public string Writer { get; set; }
    }
}