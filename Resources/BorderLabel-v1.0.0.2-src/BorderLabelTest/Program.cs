/***********************************************************************
 * BorderLabel.cs - Simple Label Control with Border Effect            *
 *                                                                     *
 *   Author:      César Roberto de Souza                               *
 *   Email:       cesarsouza at gmail.com                              *
 *   Website:     http://www.comp.ufscar.br/~cesarsouza                *
 *                                                                     *      
 *  This code is distributed under the The Code Project Open License   *
 *  (CPOL) 1.02 or any later versions of this same license. By using   *
 *  this code you agree not to remove any of the original copyright,   *
 *  patent, trademark, and attribution notices and associated          *
 *  disclaimers that may appear in the Source Code or Executable Files *
 *                                                                     *
 *  The exact terms of this license can be found on The Code Project   *
 *   website: http://www.codeproject.com/info/cpol10.aspx              *
 *                                                                     *
 ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BorderLabelTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestForm());
        }
    }
}