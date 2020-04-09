using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    abstract public class AProjectChangeProcessor : AProjectChangeNotifier
    {
        public AProjectChangeProcessor(ProjectChangeHandler handler) : base(handler) { }

        public abstract bool process(ProjectChangeEvent projectChangeEvent);

        public sealed override void start()
        {
            throw new NotImplementedException(); //that should never be called....not a good design... :/ RTODO
        }
    }
}
