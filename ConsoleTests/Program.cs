using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using DataSource;
using Utils;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            using(CDPTrackEntities context = new CDPTrackEntities())
            {
                Resource resource = new Resource
                                        {
                                            ResourceId = context.Resources.Max(r => r.ResourceId),
                                            Name = "test",
                                            DomainName = "test domain",
                                            LastLogin = DateTime.Now,
                                        };
                context.Resources.Add(resource);

                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    ErrorLogHelper.LogException(e, "CDPTracker");
                    Console.WriteLine(e);
                }
            }
        }
    }
}
