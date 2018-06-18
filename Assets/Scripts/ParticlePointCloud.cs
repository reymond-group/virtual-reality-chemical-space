using UnityEngine;
using System.Collections;

public class ParticlePointCloud : MonoBehaviour {
	public TextAsset csvFile;
	public char delimiter = ',';

	private ParticleSystem.Particle[] pointCloud;

	private bool pointCloudUpdated = false;

	void Start () {
		var lines = csvFile.text.Split ('\n');
		this.pointCloud = new ParticleSystem.Particle[lines.Length];

		for (var i = 0; i < lines.Length; i++) {
			var values = lines [i].Split (delimiter);
			if (values.Length < 3) continue;

			float x = 0.0F;
			float y = 0.0F;
			float z = 0.0F;
			float r = 0.0F;
			float g = 0.0F;
			float b = 0.0F;

			float.TryParse (values [0], out x);
			float.TryParse (values [1], out y);
			float.TryParse (values [2], out z);
			float.TryParse (values [3], out r);
			float.TryParse (values [4], out g);
			float.TryParse (values [5], out b);

			string id = values[9].Trim();

			Vector3 position = new Vector3 (x / 10.0F - 15.0F, y / 10.0F - 15.0F, z / 10.0F - 15.0F);

			this.pointCloud [i].position = position;
			this.pointCloud [i].startColor = new Color(r / 255.0F, g / 255.0F, b / 255.0F);
			this.pointCloud [i].startSize = 0.02F;

			// Molecule models can be placed at their respective coordinates here.
		}

		this.pointCloudUpdated = true;
	}
	

	void Update () {
		if (this.pointCloudUpdated) {
			GetComponent<ParticleSystem>().SetParticles (this.pointCloud, this.pointCloud.Length);
			this.pointCloudUpdated = false;
		}
	}


}
