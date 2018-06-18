using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using VRTK;

public class Atom
{
    public int Id;
    public Vector3 Position;
    public string Element;
    public List<Atom> Neighbours;

    public Atom(int id, Vector3 position, string element)
    {
        Id = id;
        Position = position;
        Element = element;
        Neighbours = new List<Atom>();
    }

    public void AddNeighbour(Atom neighbour)
    {
        this.Neighbours.Add(neighbour);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        Atom other = (Atom)obj;
        return other.Id == this.Id;
    }

    public override int GetHashCode()
    {
        return this.Id;
    }

}

public class Bond
{
    public int Id;
    public int From;
    public int To;
    public int BondCount;

    public Bond(int id, int from, int to, int bond_count)
    {
        Id = id;
        From = from;
        To = to;
        BondCount = bond_count;
    }
}

public struct Structure
{
    public Atom[] Atoms;
    public Bond[] Bonds;

    public Structure(int atom_count, int bond_count)
    {
        Atoms = new Atom[atom_count];
        Bonds = new Bond[bond_count];
    }
}

public class MoleculeLoader : MonoBehaviour
{
    public static MoleculeLoader Instance;
    private Vector3 startingPosition;
    private bool started;
    private int count = 0;

    public string DrugBankUrl = "https://www.drugbank.ca/structures/small_molecule_drugs/{0}.sdf?dim=3d";
    public TextAsset csvFile;
    public char delimiter = ',';
    public GameObject container;

    public bool Started
    {
        get
        {
            return this.started;
        }
        set
        {
            this.started = value;
        }
    }

    void Start()
    {
        Instance = this;
        StartCoroutine(this.Load());

        startingPosition = transform.localPosition;
        this.Started = false;
    }

    public void Reset()
    {
        transform.localPosition = startingPosition;
    }

    private IEnumerator Load()
    {
        var lines = csvFile.text.Split('\n');
        var loadingIndicator = GameObject.Find("Loaded");
        var loadingIndicatorText = loadingIndicator.GetComponent<UnityEngine.UI.Text>();

        for (var i = 0; i < lines.Length; i++)
        {
            var values = lines[i].Split(delimiter);
            if (values.Length < 8) continue;

            float x = 0.0F;
            float y = 0.0F;
            float z = 0.0F;
            float r = 0.0F;
            float g = 0.0F;
            float b = 0.0F;
            string id = values[9].Replace("\r", "");

            float.TryParse(values[0], out x);
            float.TryParse(values[1], out y);
            float.TryParse(values[2], out z);
            float.TryParse(values[3], out r);
            float.TryParse(values[4], out g);
            float.TryParse(values[5], out b);

            loadingIndicatorText.text = String.Format("{0:0.00} %", 100 * ((double)i / (double)lines.Length));

            yield return StartCoroutine(this.GetStructure(id, x / 10.0F - 15.0F, y / 10.0F - 15.0F, z / 10.0F - 15.0F));
        }

        loadingIndicator.SetActive(false);
    }

    private IEnumerator GetStructure(String structureId, float globalX, float globalY, float globalZ)
    {
        // yield return www;
        // WWW www = new WWW(string.Format(this.DrugBankUrl, structureId));
        String[] lines = new String[] { };
        String path = "Assets/Data/" + structureId + ".sdf";


        TextAsset textAsset = Resources.Load<TextAsset>(structureId);
        if (textAsset != null)
        {
            lines = textAsset.text.Split('\n');
        }

        if (lines.Length > 2)
        {
            // Get the atom and bond numbers
            int atom_count = 0;
            int bond_count = 0;

            string atoms = lines[3].Substring(0, 3).Trim();
            string bonds = lines[3].Substring(3, 3).Trim();

            int.TryParse(atoms, out atom_count);
            int.TryParse(bonds, out bond_count);

            Structure structure = new Structure(atom_count, bond_count);
            GameObject group = new GameObject("Molecule");

            // Loop over the atoms
            for (var i = 4; i < 4 + atom_count; i++)
            {
                string line = lines[i];

                string str_x = line.Substring(0, 10).Trim();
                string str_y = line.Substring(10, 10).Trim();
                string str_z = line.Substring(20, 10).Trim();
                string element = line.Substring(30, 3).Trim();

                float x = 0;
                float y = 0;
                float z = 0;

                float.TryParse(str_x, out x);
                float.TryParse(str_y, out y);
                float.TryParse(str_z, out z);

                int id = i - 4;
                structure.Atoms[id] = new Atom(id, new Vector3(x, y, z), element);
            }

            // Loop over the bonds
            for (var i = 4 + atom_count; i < 4 + atom_count + bond_count; i++)
            {
                string line = lines[i];

                // Get the indices of the atoms - subtract one because they start from 1
                string str_from = line.Substring(0, 3).Trim();
                string str_to = line.Substring(3, 3).Trim();
                string str_n_bonds = line.Substring(6, 3).Trim();

                int from = 0;
                int to = 0;
                int n_bonds = 0;

                int.TryParse(str_from, out from);
                int.TryParse(str_to, out to);
                int.TryParse(str_n_bonds, out n_bonds);

                int id = i - (4 + atom_count);
                structure.Bonds[id] = new Bond(id, from - 1, to - 1, n_bonds);
            }

            // At the neighbours to each atom
            foreach (var bond in structure.Bonds)
            {
                var fromAtom = structure.Atoms[bond.From];
                var toAtom = structure.Atoms[bond.To];

                fromAtom.AddNeighbour(toAtom);
                toAtom.AddNeighbour(fromAtom);
            }

            // Get the molecular weight
            var molecularWeight = "not set";
            for (var i = 4 + atom_count + bond_count; i < lines.Length; i++)
            {
                if (lines[i] == "> <MOLECULAR_WEIGHT>")
                {
                    molecularWeight = lines[i + 1];
                }
            }

            GameObject oxygens = new GameObject("Atoms");
            GameObject nitrogens = new GameObject("Atoms");
            GameObject carbons = new GameObject("Atoms");
            GameObject hydrogens = new GameObject("Atoms");
            GameObject chlorines = new GameObject("Atoms");

            var spherePrefab = Resources.Load("sphere");

            foreach (var atom in structure.Atoms)
            {
                if (atom.Element.ToLower() == "h") continue;

                // GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                GameObject sphere = (GameObject)Instantiate(spherePrefab, new Vector3(0, 0, 0), Quaternion.identity);

                sphere.transform.position = atom.Position;
                sphere.transform.localScale = new Vector3(42.0f, 42.0f, 42.0f);

                sphere.GetComponent<Renderer>().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                sphere.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                if (atom.Element.ToLower() == "o")
                {
                    sphere.GetComponent<Renderer>().material.color = Color.red;
                    sphere.transform.parent = oxygens.transform;
                }
                else if (atom.Element.ToLower() == "n")
                {
                    sphere.GetComponent<Renderer>().material.color = Color.blue;
                    sphere.transform.parent = nitrogens.transform;
                }
                else if (atom.Element.ToLower() == "c")
                {
                    sphere.GetComponent<Renderer>().material.color = Color.black;
                    sphere.transform.parent = carbons.transform;
                }
                else if (atom.Element.ToLower() == "cl")
                {
                    sphere.GetComponent<Renderer>().material.color = Color.green;
                    sphere.transform.parent = carbons.transform;
                }
                else if (atom.Element.ToLower() == "h")
                {
                    sphere.transform.localScale = new Vector3(36.0f, 36.0f, 36.0f);
                    sphere.transform.parent = hydrogens.transform;
                }
                else
                {
                    // Also add others to hydrigens for now
                    sphere.transform.parent = hydrogens.transform;
                }
                    
            }

            GameObject cylinders = new GameObject("Bonds");
            var cylinderPrefab = Resources.Load("cylinder");

            foreach (var bond in structure.Bonds)
            {
                if (structure.Atoms[bond.From].Element.ToLower() == "h" ||
                   structure.Atoms[bond.To].Element.ToLower() == "h") continue;

                var from = structure.Atoms[bond.From].Position;
                var to = structure.Atoms[bond.To].Position;

                // GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                GameObject cylinder = (GameObject)Instantiate(cylinderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                
                cylinder.GetComponent<Renderer>().material.color = Color.white;
                cylinder.GetComponent<Renderer>().material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                cylinder.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                
                
                cylinder.transform.position = (to - from) / 2.0f + from;
                cylinder.layer = 2;

                var scale = cylinder.transform.localScale;

                var bondWidth = 6.0f;

                scale.x = bondWidth;
                scale.y = bondWidth;
                scale.z = (to - from).magnitude * 32;

                cylinder.transform.localScale = scale;
                cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, to - from);
                cylinder.transform.Rotate(90, 0, 0);

                if (bond.BondCount == 2)
                {
                    // Draw the double bonds in the local plane spanned by atoms
                    var fromAtom = structure.Atoms[bond.From];
                    var toAtom = structure.Atoms[bond.To];

                    Atom neighbourAtom = null;

                    if (fromAtom.Neighbours.Count > 1)
                    {
                        neighbourAtom = fromAtom.Neighbours[0];
                        if (neighbourAtom.Id == toAtom.Id)
                        {
                            neighbourAtom = fromAtom.Neighbours[1];
                        }
                    }
                    else if (toAtom.Neighbours.Count > 1)
                    {
                        neighbourAtom = toAtom.Neighbours[0];
                        if (neighbourAtom.Id == fromAtom.Id)
                        {
                            neighbourAtom = toAtom.Neighbours[1];
                        }
                    }

                    var offset = cylinder.transform.up;

                    // Get the normal to the plane spanned by 3 atoms
                    if (neighbourAtom != null)
                    {
                        var a = neighbourAtom.Position - fromAtom.Position;
                        var b = toAtom.Position - fromAtom.Position;

                        var normal = Vector3.Cross(a, b);
                        Debug.Log("a: " + a.x + ", " + a.y + ", " + a.z + "      b: " + b.x + ", " + b.y + ", " + b.z + "      Norm: " + normal.x + ", " + normal.y + ", " + normal.z);
                        normal /= normal.magnitude;

                        // Rotate the normal around the bond by 90°
                        offset = Quaternion.AngleAxis(90, b) * normal;
                    }
                    
                    GameObject cylinder2 = (GameObject)Instantiate(cylinder, new Vector3(0, 0, 0), Quaternion.identity);
                    cylinder2.transform.position = (to - from) / 2.0f + from + offset * 0.15f;
                    cylinder2.layer = 2;
                    cylinder2.transform.rotation = Quaternion.FromToRotation(Vector3.up, to - from);
                    cylinder2.transform.Rotate(90, 0, 0);

                    cylinder2.transform.parent = cylinders.transform;

                    cylinder.transform.position -= offset * 0.15f;
                }

                cylinder.transform.parent = cylinders.transform;
            }

            /*
            oxygens.isStatic = true;
            nitrogens.isStatic = true;
            carbons.isStatic = true;
            hydrogens.isStatic = true;
            cylinders.isStatic = true;
            group.isStatic = true;

            oxygens.transform.parent = group.transform;
            nitrogens.transform.parent = group.transform;
            carbons.transform.parent = group.transform;
            hydrogens.transform.parent = group.transform;
            cylinders.transform.parent = group.transform;
            */

            // Combine Meshes

            GameObject cylindersObject = new GameObject("Bonds");
            cylindersObject.AddComponent<MeshFilter>();
            cylindersObject.AddComponent<MeshRenderer>();
            cylindersObject.GetComponent<Renderer>().material.color = Color.white;

            GameObject oxygensObject = new GameObject("Oxygens");
            oxygensObject.AddComponent<MeshFilter>();
            oxygensObject.AddComponent<MeshRenderer>();
            oxygensObject.GetComponent<Renderer>().material.color = Color.red;

            GameObject nitrogensObject = new GameObject("Nitrogens");
            nitrogensObject.AddComponent<MeshFilter>();
            nitrogensObject.AddComponent<MeshRenderer>();
            nitrogensObject.GetComponent<Renderer>().material.color = Color.blue;

            GameObject carbonsObject = new GameObject("Carbons");
            carbonsObject.AddComponent<MeshFilter>();
            carbonsObject.AddComponent<MeshRenderer>();
            carbonsObject.GetComponent<Renderer>().material.color = Color.black;

            GameObject chlorinesObject = new GameObject("Carbons");
            chlorinesObject.AddComponent<MeshFilter>();
            chlorinesObject.AddComponent<MeshRenderer>();
            chlorinesObject.GetComponent<Renderer>().material.color = Color.green;


            GameObject hydrogensObject = new GameObject("Hydrogens");
            hydrogensObject.AddComponent<MeshFilter>();
            hydrogensObject.AddComponent<MeshRenderer>();
            hydrogensObject.GetComponent<Renderer>().material.color = Color.gray;

            List<CombineInstance> cylindersCombineInstances = new List<CombineInstance>();
            List<CombineInstance> oxygensCombineInstances = new List<CombineInstance>();
            List<CombineInstance> nitrogensCombineInstances = new List<CombineInstance>();
            List<CombineInstance> carbonsCombineInstances = new List<CombineInstance>();
            List<CombineInstance> chlorinesCombineInstances = new List<CombineInstance>();
            List<CombineInstance> hydrogensCombineInstances = new List<CombineInstance>();

            MeshFilter[] cylindersMeshFilters = cylinders.GetComponentsInChildren<MeshFilter>(true);
            MeshFilter[] oxygensMeshFilters = oxygens.GetComponentsInChildren<MeshFilter>(true);
            MeshFilter[] nitrogensMeshFilters = nitrogens.GetComponentsInChildren<MeshFilter>(true);
            MeshFilter[] carbonsMeshFilters = carbons.GetComponentsInChildren<MeshFilter>(true);
            MeshFilter[] chlorinesMeshFilters = chlorines.GetComponentsInChildren<MeshFilter>(true);
            MeshFilter[] hydrogensMeshFilters = hydrogens.GetComponentsInChildren<MeshFilter>(true);

            for (int i = 0; i < cylindersMeshFilters.Length; i++)
            {
                MeshFilter meshFilter = cylindersMeshFilters[i];
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilter.sharedMesh;
                combine.transform = meshFilter.transform.localToWorldMatrix;
                cylindersCombineInstances.Add(combine);
                meshFilter.gameObject.SetActive(false);
                Destroy(meshFilter);
            }

            
            for (int i = 0; i < oxygensMeshFilters.Length; i++)
            {
                MeshFilter meshFilter = oxygensMeshFilters[i];
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilter.sharedMesh;
                combine.transform = meshFilter.transform.localToWorldMatrix;
                oxygensCombineInstances.Add(combine);
                meshFilter.gameObject.SetActive(false);
                Destroy(meshFilter);
            }

            for (int i = 0; i < nitrogensMeshFilters.Length; i++)
            {
                MeshFilter meshFilter = nitrogensMeshFilters[i];
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilter.sharedMesh;
                combine.transform = meshFilter.transform.localToWorldMatrix;
                nitrogensCombineInstances.Add(combine);
                meshFilter.gameObject.SetActive(false);
                Destroy(meshFilter);
            }

            for (int i = 0; i < carbonsMeshFilters.Length; i++)
            {
                MeshFilter meshFilter = carbonsMeshFilters[i];
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilter.sharedMesh;
                combine.transform = meshFilter.transform.localToWorldMatrix;
                carbonsCombineInstances.Add(combine);
                meshFilter.gameObject.SetActive(false);
                Destroy(meshFilter);
            }

            for (int i = 0; i < chlorinesMeshFilters.Length; i++)
            {
                MeshFilter meshFilter = chlorinesMeshFilters[i];
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilter.sharedMesh;
                combine.transform = meshFilter.transform.localToWorldMatrix;
                chlorinesCombineInstances.Add(combine);
                meshFilter.gameObject.SetActive(false);
                Destroy(meshFilter);
            }

            for (int i = 0; i < hydrogensMeshFilters.Length; i++)
            {
                MeshFilter meshFilter = hydrogensMeshFilters[i];
                CombineInstance combine = new CombineInstance();
                combine.mesh = meshFilter.sharedMesh;
                combine.transform = meshFilter.transform.localToWorldMatrix;
                hydrogensCombineInstances.Add(combine);
                meshFilter.gameObject.SetActive(false);
                Destroy(meshFilter);
            }

            cylindersObject.GetComponent<MeshFilter>().mesh = new Mesh();
            cylindersObject.GetComponent<MeshFilter>().mesh.CombineMeshes(cylindersCombineInstances.ToArray());
            cylindersObject.transform.parent = group.transform;

            oxygensObject.GetComponent<MeshFilter>().mesh = new Mesh();
            oxygensObject.GetComponent<MeshFilter>().mesh.CombineMeshes(oxygensCombineInstances.ToArray());
            oxygensObject.transform.parent = group.transform;

            nitrogensObject.GetComponent<MeshFilter>().mesh = new Mesh();
            nitrogensObject.GetComponent<MeshFilter>().mesh.CombineMeshes(nitrogensCombineInstances.ToArray());
            nitrogensObject.transform.parent = group.transform;

            carbonsObject.GetComponent<MeshFilter>().mesh = new Mesh();
            carbonsObject.GetComponent<MeshFilter>().mesh.CombineMeshes(carbonsCombineInstances.ToArray());
            carbonsObject.transform.parent = group.transform;

            chlorinesObject.GetComponent<MeshFilter>().mesh = new Mesh();
            chlorinesObject.GetComponent<MeshFilter>().mesh.CombineMeshes(chlorinesCombineInstances.ToArray());
            chlorinesObject.transform.parent = group.transform;

            hydrogensObject.GetComponent<MeshFilter>().mesh = new Mesh();
            hydrogensObject.GetComponent<MeshFilter>().mesh.CombineMeshes(hydrogensCombineInstances.ToArray());
            hydrogensObject.transform.parent = group.transform;

            group.transform.localScale = new Vector3(0.0125f, 0.0125f, 0.0125f);
            group.transform.localPosition = new Vector3(globalX, globalY, globalZ);
            group.SetActive(false);

            GameObject collisionHelper = new GameObject();
            collisionHelper.transform.localPosition = new Vector3(globalX, globalY, globalZ);

            SphereCollider collider = collisionHelper.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.0075F;

            MoleculeVisibility mv = collisionHelper.AddComponent<MoleculeVisibility>();
            mv.molecule = group;

            group.transform.parent = this.container.transform;
            collisionHelper.transform.parent = this.container.transform;

            Destroy(cylinders);
            Destroy(oxygens);
            Destroy(nitrogens);
            Destroy(carbons);
            Destroy(chlorines);
            Destroy(hydrogens);

            Resources.UnloadUnusedAssets();

            // Setup interaction
            VRTK_InteractableObject interactable = collisionHelper.AddComponent<VRTK_InteractableObject>();
            interactable.enabled = true;

            var interaction = collisionHelper.AddComponent<MoleculeInteraction>();
            interaction.text = "ID: " + structureId + "\nMol. Weight: " + molecularWeight;

            yield return new WaitUntil(() => true);
        }
    }
}
