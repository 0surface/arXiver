using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.SubjectAggregate
{
    public class Discipline : Enumeration
    {
        public static Discipline Astrophysics = new Discipline(1, "Astrophysics", "astro-ph",ScientificField.Physics);
        public static Discipline CondensedMatter = new Discipline(2, "Condensed Matter", "cond-mat", ScientificField.Physics);
        public static Discipline GeneralRelativityQuantumCosmology = new Discipline(3, "General Relativity and Quantum Cosmology", "gr-qc", ScientificField.Physics);
        public static Discipline HighEnergyPhysicsExperiment = new Discipline(4, "High Energy Physics - Experiment", "hep-ex", ScientificField.Physics);
        public static Discipline HighEnergyPhysicsLattice = new Discipline(5, "High Energy Physics - Lattice", "hep-lat", ScientificField.Physics);
        public static Discipline HighEnergyPhysicsPhenomenology  = new Discipline(6, "High Energy Physics - Phenomenology", "hep-ph", ScientificField.Physics);
        public static Discipline MathematicalPhysics = new Discipline(7, "Mathematical Physics", "math-ph", ScientificField.Physics);
        public static Discipline NonlinearSciences  = new Discipline(8, "Nonlinear Sciences", "nlin", ScientificField.Physics); 
        public static Discipline NuclearExperiment = new Discipline(9, "Nuclear Experiment", "nucl-ex", ScientificField.Physics);
        public static Discipline NuclearTheory = new Discipline(10, "Nuclear Theory", "nucl-th", ScientificField.Physics);
        public static Discipline Physics  = new Discipline(11, "Physics", "physics", ScientificField.Physics);
        public static Discipline QuantumPhysics  = new Discipline(12, "Quantum Physics", "quant-ph", ScientificField.Physics);

        public static Discipline Mathematics = new Discipline(13, "Mathematics", "math", ScientificField.Mathematics);
        public static Discipline ComputerScience = new Discipline(14, "Computer Science", "cs", ScientificField.ComputerScience);
        public static Discipline QuantitativeBiology = new Discipline(15, "Quantitative Biology", "q-bio", ScientificField.QuantitativeBiology);
        public static Discipline QuantitativeFinance = new Discipline(16, "Quantitative Finance", "q-bio", ScientificField.QuantitativeFinance);
        public static Discipline Statistics = new Discipline(17, "Statistics", "stat", ScientificField.Statistics);
        public static Discipline ElectricalEngineeringSystemsScience = new Discipline(18, "Electrical Engineering and Systems Science", "eess ", ScientificField.ElectricalEngineeringSystemsScience);
        public static Discipline Economics = new Discipline(19, "Economics", "econ", ScientificField.Economics);

        public ScientificField Field { get; set; }
        public string Code { get; set; }

        public Discipline(int id, string name, string code, ScientificField field) 
            : base(id, name)
        {

        }
    }
}

