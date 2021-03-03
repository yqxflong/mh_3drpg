using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ResCheckResult
{
    List<string> _errorMessages = null;
    private string _checkName;

    public List<string> errorMessages
    {
        get
        {

            return _errorMessages;
        }
    }
    public string checkName
    {
        get
        {
            return _checkName;
        }
    }
    public bool hasError
    {
        get
        {
            return errorMessages != null && errorMessages.Count > 0;
        }
    }
    public ResCheckResult(string checkName_, List<string> errors = null)
    {
        _checkName = checkName_;
        _errorMessages = errors;
    }

    public readonly static ResCheckResult Default = new ResCheckResult("Default");

}
