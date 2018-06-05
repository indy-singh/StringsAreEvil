namespace StringsAreEvil
{
    public class ValueHolder
    {
        public int ElementId { get; }
        public int VehicleId { get; }
        public int Term { get; }
        public int Mileage { get; }
        public decimal Value { get; }

        public ValueHolder(string line)
        {
            var parts = line.Split(',');

            ElementId = int.Parse(parts[1]);
            VehicleId = int.Parse(parts[2]);
            Term = int.Parse(parts[3]);
            Mileage = int.Parse(parts[4]);
            Value = decimal.Parse(parts[5]);
        }

        public ValueHolder(string[] parts)
        {
            ElementId = int.Parse(parts[1]);
            VehicleId = int.Parse(parts[2]);
            Term = int.Parse(parts[3]);
            Mileage = int.Parse(parts[4]);
            Value = decimal.Parse(parts[5]);
        }

        public ValueHolder(int elementId, int vehicleId, int term, int mileage, decimal value)
        {
            ElementId = elementId;
            VehicleId = vehicleId;
            Term = term;
            Mileage = mileage;
            Value = value;
        }

        public override string ToString()
        {
            return ElementId + "," + VehicleId + "," + Term + "," + Mileage + "," + Value;
        }
    }

    public struct ValueHolderAsStruct
    {
        public int ElementId { get; }
        public int VehicleId { get; }
        public int Term { get; }
        public int Mileage { get; }
        public decimal Value { get; }

        public ValueHolderAsStruct(int elementId, int vehicleId, int term, int mileage, decimal value)
        {
            ElementId = elementId;
            VehicleId = vehicleId;
            Term = term;
            Mileage = mileage;
            Value = value;
        }

        public override string ToString()
        {
            return ElementId + "," + VehicleId + "," + Term + "," + Mileage + "," + Value;
        }
    }
}