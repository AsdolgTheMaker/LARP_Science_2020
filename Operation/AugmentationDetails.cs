using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LARP.Science.Database;
using AsdolgTools;

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

        protected async override Task<bool> Process()
        {
            switch (type)
            {
                case AugmentationType.Organ:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    Economics.Exchange.TakeItem((implant as Organ).ConvertToEjectedOrgan());
                                    bool successful = Patient.InstallOrgan((implant as Organ).Slot);
                                    if (successful) Journal.AddRecord("Орган \"" + implant.Name + "\" установлен в пациента " + Patient.Name + ".", Controller.LogOutputDuringOperation);
                                    else return false;
                                    return true;
                                }
                            case AugmentationAction.Remove:
                                {
                                    Organ ejected = Patient.EjectOrgan((target as Organ).Slot);
                                    if (ejected != null)
                                    {

                                        Economics.Exchange.AddItem(ejected);
                                        Journal.AddRecord("Из пациента" + Patient.Name + " извлечён орган \"" + ejected.Name + "\" и убран на склад.", Controller.LogOutputDuringOperation);
                                        return true;
                                    }
                                    else return false;
                                }
                            default: return false;
                        }
                    }
                case AugmentationType.Primary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    Augment ejected = Patient.InstallAugmentToOrganSlot(implant as Augment);
                                    Economics.Exchange.TakeItem((implant as Augment).ConvertToEjectedAugment());
                                    if ((implant as Augment).IsReplacement)
                                        Journal.AddRecord("Протез \"" + implant.Name + "\" органа \"" + target.Name + "\" установлен в пациента " + Patient.Name + ".", Controller.LogOutputDuringOperation);
                                    else
                                    {
                                        Journal.AddRecord("Аугмент \"" + implant.Name + "\" для органа \"" + target.Name + "\" установлен в пациента " + Patient.Name + ".", Controller.LogOutputDuringOperation);
                                        if (ejected != null)
                                        {
                                            Economics.Exchange.AddItem(ejected);
                                            Journal.AddRecord("Взамен, аугмент \"" + ejected.Name + "\" извлечён и убран на склад.", Controller.LogOutputDuringOperation);
                                        }
                                    }
                                    return true;
                                }
                            case AugmentationAction.Remove:
                                {
                                    Augment ejected = Patient.EjectAugmentFromOrganSlot((target as Organ).Slot);
                                    if (ejected != null)
                                    {
                                        Economics.Exchange.AddItem(ejected);
                                        if (ejected.IsReplacement) Journal.AddRecord("Протез \"" + ejected.Name + "\" органа \"" + ((Character.BodyPartSlot.SlotType)(ejected.Slot)).GetDescription() + "\" извлечён из пациента " + Patient.Name + " и убран на склад.", Controller.LogOutputDuringOperation);
                                        else Journal.AddRecord("Аугмент \"" + ejected.Name + "\" для органа \"" + ((Character.BodyPartSlot.SlotType)(ejected.Slot)).GetDescription() + "\" извлечён из пациента " + Patient.Name + " и убран на склад.", Controller.LogOutputDuringOperation);
                                        return true;
                                    }
                                    else return false;
                                }
                            default: return false;
                        }
                    }
                case AugmentationType.Auxilary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                {
                                    Economics.Exchange.TakeItem((implant as Augment).ConvertToEjectedAugment());
                                    Patient.AddAugment(implant as Augment);
                                    Journal.AddRecord("Аугмент \"" + implant.Name + "\" успешно имплантирован в пациента" + Patient.Name + ".", Controller.LogOutputDuringOperation);
                                    return true;
                                }
                            case AugmentationAction.Remove:
                                {
                                    bool didEject = Patient.EjectAugment(target as Augment);
                                    if (didEject) Journal.AddRecord("Извлечён имплант " + target.Name + " из пациента " + Patient.Name, Controller.LogOutputDuringOperation);
                                    return didEject;
                                }
                            default: return false;
                        }
                    }
                default: return false;
            }
        }

        protected override void OnStart()
        {

        }

        protected override void OnFail()
        {

        }

        protected override void OnSuccess()
        {

        }

        protected override void OnFinished()
        {

        }

        /// <summary>
        /// Набор агрументов зависит от желаемого типа операции: <br/><br/>
        /// Добавить орган            - Передать Organ как IMPLANT <br/> 
        /// Аугментировать            - Передать Augment как IMPLANT <br/>
        /// Удалить орган или аугмент - Передать Organ как TARGET <br/>
        /// Деимплантировать          - Передать Augment как TARGET
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
        /// Validity pass to be assured that passed arguments correspond to constructor instructions.
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
                                if (!(implant is Organ)) throw new ArgumentException();
                                break;
                            case AugmentationAction.Remove:
                                if (!(target is Organ)) throw new ArgumentException();
                                break;
                        }
                        break;
                    }
                case AugmentationType.Primary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                if (!(implant is Augment)) throw new ArgumentException();
                                break;
                            case AugmentationAction.Remove:
                                if (!(target is Organ)) throw new ArgumentException();
                                break;
                        }
                        break;
                    }
                case AugmentationType.Auxilary:
                    {
                        switch (action)
                        {
                            case AugmentationAction.Install:
                                if (!(implant is Augment)) throw new ArgumentException();
                                    break;
                            case AugmentationAction.Remove:
                                if (!(target is Augment)) throw new ArgumentException();
                                break;
                        }
                        break;
                    }
            }
        }
    }
}
