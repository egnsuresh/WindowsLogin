using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;

using System.Web.Http;
using System.Web.Security;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using System.Web.Http.Cors;
using System.IO;
using System.Configuration;
namespace WindowsLoginPage.Controllers
{
 // SupportsCredentials is mandatory for windows login
      [EnableCors(origins: "http://abcd.com:8001,http://pqrs.com:8001", headers: "*",methods: "*", SupportsCredentials = true)]
      [RoutePrefix("api/Account")] 
  public class AccountController : ApiController
    {
      public AccountController()
        {
        }
       [System.Web.Http.Route("UserName")]
        public IHttpActionResult GetUserInfo2()
        {
            string ntid = "";
            string token=""+System.Web.HttpContext.Current.Request.LogonUserIdentity.Token;
            return Ok(new { ntid = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\')[1]  });
        }
      //for file upload
       [HttpPost]
        [Route("upload")]
        public IHttpActionResult Upload(string ntid = "nontid")
        {
          string path = "";
            string uniquePath = DateTime.Now.ToString("yyyyMMddhhmmss");
            try
            {
                System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
                if (hfc.Count <= 0)
                {
                    throw new Exception("file count is zero");
                }
                path = ConfigurationManager.AppSettings["fileUploadFolder"];
                path += "\\" + (ntid + uniquePath);
                if (!(Directory.Exists(path)))
                    Directory.CreateDirectory(path);

                
                HttpResponseMessage response = new HttpResponseMessage();
                var httpRequest = System.Web.HttpContext.Current;
                string[] paths = new string[httpRequest.Request.Files.Count];
                if (httpRequest.Request.Files.Count > 0)
                {
                    //Loop through uploaded files  
                    for (int i = 0; i < httpRequest.Request.Files.Count; i++)
                    {
                        HttpPostedFile httpPostedFile = httpRequest.Request.Files[i];
                        if (httpPostedFile != null)
                        {
                            // Construct file save path  
                            var fileSavePath = Path.Combine(path, httpPostedFile.FileName);
                            paths[i] = fileSavePath;
                            // Save the uploaded file  
                            httpPostedFile.SaveAs(fileSavePath);
                        }
                    }
                    return Ok(new { attachmentsPath = path });
                }
                else
                {
                    return Ok(new { attachmentsPath = "files count is zero" });
                }

                
            }
            catch (Exception e)
            {
                return BadRequest("error occured or not able to save the files due to following reason " + e.Message + " user name:" + System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.Split('\\')[1]);

            }
        }
     }

}
