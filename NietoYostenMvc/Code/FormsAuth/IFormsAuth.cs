using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NietoYostenMvc.Code.FormsAuth
{
    public interface IFormsAuth
    {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        void SignOut();
    }
}
