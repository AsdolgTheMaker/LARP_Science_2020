using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    public class Damage
    {
        public DamageSource damageSource { get; }

        public enum DamageSource
        {
            Blaster,
            Saber,
            Undefined
        }

        public Damage(DamageSource source)
        {
            damageSource = source;

            switch (damageSource)
            {
                case DamageSource.Blaster:
                    {
                        break;
                    }
                case DamageSource.Saber:
                    {
                        break;
                    }
                case DamageSource.Undefined:
                    {
                        break;
                    }
            }
        }
    }
}
