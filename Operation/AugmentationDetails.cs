using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LARP.Science.Database;

namespace LARP.Science.Operation
{
    /// <summary>
    /// Create the instance first, call PerformOperation() method next.
    /// </summary>
    public class AugmentationDetails : OperationInfo
    {
        private readonly AugmentationType type;
        private readonly AugmentationAction action;
        private readonly BodyPart target;
        private readonly BodyPart implant;

        public override void PerformOperation() 
        {
            switch (type)
            {
                case AugmentationType.Organ:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    Patient.InstallOrgan(implant as Organ);
                                    break;
                                }
                            case AugmentationAction.Remove:
                                {
                                    Patient.EjectOrgan((target as Organ).Slot);
                                    break; 
                                }
                        }
                        break;
                    }
                case AugmentationType.Primary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    Patient.InstallAugmentToOrganSlot(implant as Augment);
                                    break;
                                }
                            case AugmentationAction.Remove:
                                {
                                    Patient.EjectAugmentFromOrganSlot((target as Organ).Slot);
                                    break;
                                }
                        }
                        break;
                    }
                case AugmentationType.Auxilary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    Patient.AddAugment(implant as Augment);
                                    break;
                                }
                            case AugmentationAction.Remove:
                                {
                                    Patient.EjectAugment(target as Augment);
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Args depend on desired operation: <br/><br/>
        /// Add Organ               - Pass Organ as IMPLANT <br/> 
        /// Add Primary/Auxilary    - Pass Augment as IMPLANT <br/>
        /// Delete Organ/Primary    - Pass Organ as TARGET <br/>
        /// Delete Auxilary         - Pass Augment as TARGET
        /// </summary>
        public AugmentationDetails(AugmentationType _type, AugmentationAction _action, BodyPart _target = null, BodyPart _implant = null)
        {
            type = _type;
            action = _action;
            target = _target;
            implant = _implant;

            Validate();
        }

        /// <summary>
        /// Validity pass to be assured that passed arguments correspond to Augmentation constructor instructions.
        /// </summary>
        private void Validate()
        {
            switch (type)
            {
                case AugmentationType.Organ:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    if (!(implant is Organ)) throw new ArgumentException();
                                    break;
                                }
                            case AugmentationAction.Remove:
                                {
                                    if (!(target is Organ)) throw new ArgumentException();
                                    break;
                                }
                        }
                        break; 
                    }
                case AugmentationType.Primary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    if (!(implant is Augment)) throw new ArgumentException();
                                    break;
                                }
                            case AugmentationAction.Remove:
                                {
                                    if (!(target is Organ)) throw new ArgumentException();
                                    break;
                                }
                        }
                        break;
                    }
                case AugmentationType.Auxilary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    if (!(implant is Augment)) throw new ArgumentException();
                                    break;
                                }
                            case AugmentationAction.Remove:
                                {
                                    if (!(target is Augment)) throw new ArgumentException();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }
    }
}
