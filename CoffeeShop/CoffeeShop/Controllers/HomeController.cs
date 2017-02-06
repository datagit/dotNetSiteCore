using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CoffeeShop.Models;
using Glass.Mapper.Sc;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Form.Core.Controls.Data;
using Sitecore.Form.Core.Data;
using Sitecore.Form.Core.Submit;
using Sitecore.Form.Web.UI.Controls;
using Sitecore.Links;
using Sitecore.SecurityModel;
using Log = Sitecore.Diagnostics.Log;

namespace CoffeeShop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HomeViewModel result = null;
            var item = Sitecore.Context.Item;
            if (item != null)
            {
                result = new HomeViewModel
                {
                    Title = item.Fields["Title"].Value,
                    Description = item.Fields["Description"] == null ? string.Empty : item.Fields["Description"].Value

                };
                var isShowBanner = item.Fields["Show Banner"].Value.Equals("1");
                if (isShowBanner)
                {
                    MultilistField bannerField = item.Fields["Banner"];
                    result.BannerImage = bannerField == null
                        ? null
                        : bannerField.GetItems().Where(c => ((MediaItem)c) != null).Select(s => new ImageViewModel
                        {
                            AlternateText = ((MediaItem)s).Alt,
                            SourceUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(s)
                        }).ToList();
                }
            }
            return View(result);
        }

        //public ActionResult Index()
        //{
        //    var context = new SitecoreContext();
        //    var result = context.GetCurrentItem<GlassHomeViewModel>();
        //    if (!result.IsShowBanner)
        //    {
        //        result.Banner = null;
        //    }
        //    return View(result);
        //}

        public ActionResult Blog()
        {
            var result = new List<BlogViewModel>();
            using (new SecurityDisabler())
            {
                var blogs = Sitecore.Context.Database.SelectItems("fast:/sitecore/content/CoffeeShop/Blog//*[@@templatename='Blog']");
                if (blogs != null && blogs.Any())
                {
                    result = blogs.Where(c => ((ImageField)c.Fields["Image"]).MediaItem != null).OrderByDescending(o => ((DateField)o.Fields["Posted Date"]).DateTime.ToLocalTime()).Select(s =>
                            {
                                ImageField imageField = s.Fields["Image"];

                                return new BlogViewModel
                                {
                                    Title = s.Fields["Title"].Value,
                                    LinkUrl = string.Format("/detail?blog={0}", (new Guid(s.ID.ToString())).ToString("D")),
                                    CreatedDate = ((DateField)s.Fields["Posted Date"]).DateTime.ToLocalTime().ToString("MMMM dd, yyyy"),
                                    ShortDescription = s.Fields["Short Description"] == null ? string.Empty : s.Fields["Short Description"].Value,
                                    BlogImage = new ImageViewModel
                                    {
                                        AlternateText = imageField.Alt,
                                        SourceUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageField.MediaItem)
                                    }
                                };
                            }).ToList();
                }
            }
            return View(result);
        }

        public ActionResult Detail(string blog = "")
        {
            if (string.IsNullOrEmpty(blog))
            {
                return View("_Error");
            }
            var blogItem = Sitecore.Context.Database.GetItem(new ID(blog));
            if (blogItem == null)
            {
                return View("_Error");
            }
            ImageField imageField = blogItem.Fields["Image"];
            MultilistField postsField = blogItem.Fields["Posts"];
            var result = new BlogViewModel
            {
                Title = blogItem.Fields["Title"].Value,
                BlogImage = imageField.MediaItem == null ? null : new ImageViewModel
                {
                    AlternateText = imageField.Alt,
                    SourceUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(imageField.MediaItem)
                },
                Posts = postsField == null ? null : postsField.GetItems().Select(s => new PostViewModel
                {
                    Title = s.Fields["Title"].Value,
                    Description = s.Fields["Description"].Value,
                    IntroImage = ((ImageField)s.Fields["Image"]).MediaItem == null ? null : new ImageViewModel
                    {
                        AlternateText = ((ImageField)s.Fields["Image"]).Alt,
                        SourceUrl = Sitecore.Resources.Media.MediaManager.GetMediaUrl(((ImageField)s.Fields["Image"]).MediaItem)
                    }
                }).ToList(),
                Writer = blogItem.Fields["Writer"].Value
            };
            return View(result);
        }

        public ActionResult Navigation()
        {
            var result = new List<NavigationViewModel>();
            using (new SecurityDisabler())
            {
                var items = Sitecore.Context.Database.GetItem("/sitecore/content/CoffeeShop/Page");
                if (items != null && items.GetChildren().Any())
                {
                    result = items.GetChildren().Where(
                            c => c.Fields["Show in Navigation"] != null && c.Fields["Show in Navigation"].Value.Equals("1")).Select(s =>
                                {
                                    var linkItem = LinkManager.GetItemUrl(s, new UrlOptions
                                    {
                                        UseDisplayName = true,
                                        AlwaysIncludeServerUrl = true,
                                        LowercaseUrls = true,
                                        LanguageEmbedding = LanguageEmbedding.Never
                                    });
                                    if (linkItem != null)
                                    {
                                        return new NavigationViewModel
                                        {
                                            DisplayName = s.DisplayName,
                                            Url = linkItem
                                        };
                                    }
                                    return null;
                                }).Where(c => c != null).ToList();
                }
            }
            return View(result);
        }

        public ActionResult Contact()
        {
            var model = new ContactViewModel { FormId = "{6A69E28B-40EB-4590-903E-C39A9431B750}" };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var formItem = Sitecore.Context.Database.GetItem(new ID(model.FormId));
                if (formItem != null)
                {
                    //var acrList = new List<AdaptedControlResult>();
                    var cList = new List<ControlResult>();
                    var fieldItems = formItem.Axes.GetDescendants();
                    foreach (var fieldItem in fieldItems)
                    {
                        switch (fieldItem.ID.ToString())
                        {
                            case "{7B0D36D6-B80E-4869-B971-83949909318E}":
                                cList.Add(new ControlResult(fieldItem.ID.ToString(), fieldItem.Name, model.Name, string.Empty));
                                break;
                            case "{0D620B2C-C078-4C23-9E71-CE6206D059A3}":
                                cList.Add(new ControlResult(fieldItem.ID.ToString(), fieldItem.Name, model.Email ?? string.Empty, string.Empty));
                                break;
                            case "{5FAA6D35-9209-4757-92A1-4CD7E0E7D8DB}":
                                cList.Add(new ControlResult(fieldItem.ID.ToString(), fieldItem.Name, model.PhoneNumber ?? string.Empty, string.Empty));
                                break;
                            case "{00C01B93-A8C6-431E-AA37-D79026447CE0}":
                                cList.Add(new ControlResult(fieldItem.ID.ToString(), fieldItem.Name, model.Message ?? string.Empty, string.Empty));
                                break;
                        }
                    }

                    if (cList.Count > 0)
                    {
                        var simpleForm = new SitecoreSimpleForm(formItem);
                        var saveAction = simpleForm.FormItem.SaveActions;
                        var actionList = Sitecore.Form.Core.ContentEditor.Data.ListDefinition.Parse(saveAction);

                        var actionDefinitions = new List<ActionDefinition>();
                        actionDefinitions.AddRange(actionList.Groups.SelectMany(x => x.ListItems).Select(li => new ActionDefinition(li.ItemID, li.Parameters) { UniqueKey = li.Unicid }));

                        //var arl = new AdaptedResultList(acrList);

                        try
                        {
                            //SubmitActionManager.SaveFormToDatabase(formItem.ID, arl);
                            SubmitActionManager.Execute(formItem.ID, cList.ToArray(), actionDefinitions.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Submit Contact Error:", ex);
                            return Json(new { error = true });
                        }
                    }
                    return Json(new { });
                }
            }
            return Json(new { error = true });
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult methodAjax() {
            return Json(new { foo = "bar", baz = "Blech" });
        }

        public ActionResult About1() {
            HomeViewModel result = null;
            var item = Sitecore.Context.Item;
            if (item != null) {
                result = new HomeViewModel {
                    Title = item.Fields["Title"].Value,
                    Description = item.Fields["Description"] == null ? string.Empty : item.Fields["Description"].Value

                };
                
            }
            return View(result);
        }


    }
}