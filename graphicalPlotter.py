import json
import sys
import matplotlib.pyplot as plt

y = json.loads(sys.argv[1])

x = [i for i in range(1, len(y) + 1)]

plt.plot(x, y)

plt.xlabel('States')
plt.ylabel('Fitness')

plt.title('Knapsack problem')

plt.show()
# isso eh um teste do Guedes