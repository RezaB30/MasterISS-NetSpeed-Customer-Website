using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.ViewModels.Supports
{
    public class SupportTypes
    {

    }
    public enum ChangeType  // son değişikliği yapan
    {
        Customer = 1,
        Agent = 2
    }
    public enum SupportRequestDisplayTypes
    {
        NoneDisplay = 1,
        OpenRequestAgainDisplay = 2,
        AddNoteDisplay = 3
    }
    public enum SupportMesssageTypes
    {
        AddNote = 1,
        ProblemSolved = 2,
        OpenRequestAgain = 3
    }
    public enum SupportRequestStateID
    {
        InProgress = 1,
        Done = 2
    }
}
