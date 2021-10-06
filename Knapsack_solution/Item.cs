namespace Knapsack_solution {
    public class Item {
        public string Name { get; set; }
        public float weight { get; set; }
        public float value { get; set; }

        public Item(string Name, float weight, float value) {
            this.Name = Name;
            this.weight = weight;
            this.value = value;
        }

        public override bool Equals(object o) {
            if (o != null) {
                return Name == ((Item) o).Name;
            }

            return false;
        }
    }
}