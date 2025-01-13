using TransactionService.Domain.Organization;
using System.Collections.Generic;
using System.Reflection;
using DStack.Aggregates;

namespace TransactionService.App;

public class AggregateInteractorsExtractor
{
    public static Dictionary<Type, Type> GetInteractors()
    {
        var ret = new Dictionary<Type, Type>();
        var assembly = Assembly.GetAssembly(typeof(OrganizationInteractor));
        var data = assembly.GetTypes().Where(p => typeof(IInteractor).IsAssignableFrom(p) && p.IsClass).ToList();
        foreach (var t in data)
        {
            var itype = t.GetInterfaces().Where(x => x.Name != "IInteractor").First();
            ret.Add(itype, t);
        }

        return ret;
    }
}