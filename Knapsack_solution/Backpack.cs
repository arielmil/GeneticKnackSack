using System;
using System.Globalization;
using System.Linq;

namespace Knapsack_solution {
    public class Backpack {
        public int itemsQuantity { get; private set; }
        public float capacity { get; } = 77.7f;
        public float currentWeight { get; private set; } = 0.0f;
        public float currentValue { get; private set; } = 0.0f;
        public Item[] Items { get; private set; }

        private static readonly string[] ItemsNames = {
            "Medieval armor", "Television", "Hammer", "Notebook", "Toaster", "Meat", "Watermelon", "Coconut",
            "Surf Board", "Skate Board", "Electric warmer", "Cup", "Pencil case", "Canvas", "Sandwich", 
            "Lettuce", "Tomato", "Cellphone", "Cellphone Charger", "Bread", "Butter", "How to Win Friends and Influence People"
        };

        private float[] ItemsWeights = {30.0f, 22.0f, 15.0f, 12.9f, 11.5f, 10.9f, 7.8f, 7.5f
            ,7.2f, 6.4f, 6.3f, 4.3f, 3.2f, 3.0f, 2.5f,
            2.2f, 2.0f, 1.5f, 1.3f, 1.0f, 0.9f, 0.5f
        };

        private float[] ItemsValues = {
            20.0f, 19.0f, 15.0f, 19.0f, 5.0f, 9.0f, 4.5f, 5.9f, 
            2.4f, 3.4f, 9.1f, 0.5f, 2.1f, 2.1f, 3.0f, 0.5f, 0.3f, 
            15.0f, 15.0f, 9.5f, 7.8f, 5.5f
        };
        
        public Backpack() {
            itemsQuantity = 0;
            currentWeight = 0.0f;
            currentValue = 0.0f;
            Items = new Item[22];
        }

        public bool IsItemInBackpack(Item item) {
            return Items.Contains(item);
        }

        public void pack(string itemName) {
            int pos;
            pos = getItemNameInItemmsNames(itemName);
            
            if (pos != -1) {
                Items[pos] = new Item(itemName, ItemsWeights[pos], ItemsValues[pos]);
                currentWeight = currentWeight + Items[pos].weight;
                currentValue = currentValue + Items[pos].value;
            }
            else {
                throw (new Exception("Error: Invalid item name !"));
            }
        }

        public void unpack(string itemName) {
            int pos;
            pos = Array.IndexOf(ItemsNames, itemName);
            if (pos != -1) {
                Item i = Items[pos];
                i.Name = "None";
                i.weight = 0.0f;
                i.value = 0.0f;
                
                currentWeight = currentWeight - Items[pos].weight;
                currentValue = currentValue - Items[pos].value;
            }
            else {
                throw (new Exception("Error: Invalid item name !"));
            }
        }

        private static int getItemNameInItemmsNames(string itemName) {
            return Array.IndexOf(ItemsNames, itemName);
        }

        public static string getNameInPosition(int pos) {
            if (pos >= 0 && pos <= 22) {
                return ItemsNames[pos];
            }
            else {
                throw (new Exception("Error: Invalid position for Items names array !"));
            }
        }
    }
    
}