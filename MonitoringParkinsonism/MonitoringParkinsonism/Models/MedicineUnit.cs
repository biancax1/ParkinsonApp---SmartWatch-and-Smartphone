using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringParkinsonism.Models
{
    public enum MedicineUnit
    {
        NumberOfTablets = 0,
        Milliter = 1,
        Drops = 2,
        Miligram = 3,
        NumberOfPuffs = 4, 
        NumberOfSprays = 5,
        Centimeters = 6,
        NumberOfSuppositiories = 7, 

    }

    public static class MedicineUnitFunctions
    {

        public static string GetTranslatedString(MedicineUnit medicineUnit)
        {
            switch (medicineUnit)
            {
                case MedicineUnit.NumberOfTablets:
                    return "tabliet";
                case MedicineUnit.Milliter:
                    return "mililitrov";
                case MedicineUnit.Drops:
                    return "kvapiek";
                case MedicineUnit.Miligram:
                    return "miligramov";
                case MedicineUnit.NumberOfPuffs:
                    return "vdychov";
                case MedicineUnit.NumberOfSprays:
                    return "vstrekov";
                case MedicineUnit.Centimeters:
                    return "centimetrov";
                case MedicineUnit.NumberOfSuppositiories:
                    return "čapíkov";
                default:
                    return "";
            }
        }
    }

}
