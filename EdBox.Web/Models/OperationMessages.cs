using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EdBox.Core.EnumLib;

namespace EdBox.Web.Models
{
    public class OperationMessages
    {
        public DateTime MessageDate => DateTime.Now;
        public FileProcessingMessageTypes FileProcessingMessageTypes { get; set; }
        public FileProcessingState FileProcessingState { get; set; }
        public string FileProcessingMessageTypesMessage => FileProcessingMessageTypes.DisplayName();
        public string FileProcessingStateMessage => FileProcessingState.DisplayName();
        public string Message { get; set; }
    }
}