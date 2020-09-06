[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
public class RPGFunctionAlias : System.Attribute
{

    public string Alias
    {
        get;
    }

    public RPGFunctionAlias(string alias) 
    {
        this.Alias = alias;
    }

}
