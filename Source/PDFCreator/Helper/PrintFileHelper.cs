﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using pdfforge.PDFCreator.PrintFile;

namespace pdfforge.PDFCreator.Helper
{
    /// <summary>
    /// The PrintFileHelper implements the  PrintFileHelperBase functionalities without user interaction
    /// </summary>
    internal class PrintFileHelper : PrintFileHelperBase
    {
        protected override void DirectoriesNotSupportedHint()
        {
            //do nothing here
            //show message window in in PrintFileAssistant
        }

        protected override void UnprintableFilesHint(IList<PrintCommand> unprintable)
        {
            //do nothing here
            //show message window in in PrintFileAssistant
        }

        protected override bool QuerySwitchDefaultPrinter()
        {
            return false; 
            //-> No interaction
            //Ask user in PrintFileAssitant
        }
    }
}
