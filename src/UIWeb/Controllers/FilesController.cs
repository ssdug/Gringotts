using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac.Extras.NLog;
using Wiz.Gringotts.UIWeb.Data;

namespace Wiz.Gringotts.UIWeb.Controllers
{
    public class FilesController : Controller
    {
        private readonly ApplicationDbContext context;
        public ILogger Logger { get; set; }

        public FilesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ActionResult> Index(int id)
        {
            Logger.Trace("Index::{0}", id);

            var file = await context.Files
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null)
                return HttpNotFound();

            return File(file.Content, file.ContentType);
        }
    }
}