#region "copyright"

/*
    Copyright Dale Ghent <daleg@elemental.org>
    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at http://mozilla.org/MPL/2.0/
*/

#endregion "copyright"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCOM.EnhancedCanonEF {

    internal class Utility {

        internal static List<string> GetFocalRatios(string lensModel) {
            var focalRatios = new List<string>();

            StreamReader streamReader = File.OpenText(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86) + @"\ASCOM\Focuser\ASCOM.EnhancedLens.Controller\lens.txt");
            string text;
            while ((text = streamReader.ReadLine()) != null) {
                if (text.Substring(0, text.IndexOf('|') - 1) == lensModel) {
                    string text2 = text.Substring(text.IndexOf('|') + 2, text.Length - 1 - (text.IndexOf('|') + 1));
                    string[] array = text2.Split(' ', ',', ':', '\t');
                    string[] array2 = array;

                    foreach (string text3 in array2) {
                        var entry = "f/" + text3;
                        focalRatios.Add(entry);
                    }
                }
            }
            streamReader.Close();

            return focalRatios;
        }
    }
}