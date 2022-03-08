using System.Collections.Generic;
using Impingement.enums;

namespace Impingement.Stats
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiers(enumStats stat);
        IEnumerable<float> GetPercentageModifiers(enumStats stat);
    }
}