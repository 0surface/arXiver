using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.SubjectAggregate
{
    public class ScientificField : Enumeration
    {
        public static ScientificField Physics = new ScientificField(1, "Physics");
        public static ScientificField Mathematics = new ScientificField(2, "Mathematics");
        public static ScientificField ComputerScience = new ScientificField(3, "Computer Science");
        public static ScientificField QuantitativeBiology = new ScientificField(4, "Quantitative Biology");
        public static ScientificField QuantitativeFinance = new ScientificField(5, "Quantitative Finance");
        public static ScientificField Statistics = new ScientificField(6, "Statistics");
        public static ScientificField ElectricalEngineeringSystemsScience = new ScientificField(7, "Electrical Engineering and Systems Science");
        public static ScientificField Economics = new ScientificField(8, "Economics");

        public ScientificField(int id, string name)
            : base (id,name)
        {
        }
    }
}
