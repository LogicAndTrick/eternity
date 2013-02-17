using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eternity.Controls.Easings
{
    public interface IEasing
    {
        double CalculateEasing(double timeProgress);
    }
}
