import os.path
import urllib.request

import pandas as pd

X = pd.read_csv('DrugBank_merge.ringc.csv', header=None)

for index, row in X.iterrows():
    if index < 4228 or os.path.isfile('https://www.drugbank.ca/structures/small_molecule_drugs/{}.sdf?dim=3d'.format(row[9])):
        continue
    try:
        urllib.request.urlretrieve(
            'https://www.drugbank.ca/structures/small_molecule_drugs/{}.sdf?dim=3d'.format(row[9]), str(row[9]) + '.sdf')
    except:
        print(404)
