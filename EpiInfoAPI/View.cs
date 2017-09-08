using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpiInfoAPI
{
   public class View
    {
        #region Fields
        private string name = string.Empty;
        private bool isRelatedView = false;
        private Project project;
        #endregion

        /// <summary>
        /// Gets a reference to the Epi7 project object.
        /// </summary>
        /// <returns></returns>
        public virtual Project GetProject()
        {
            return project;
        }
    }
}
