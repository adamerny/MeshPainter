using UnityEngine;
using System.Collections;

public class MeshPainter : MonoBehaviour {
	float GridSizeWidth = 10;
	float GridSizeHeight = 10;
	float GridSectionWidth = .1f;
	float GridSectionHeight = .1f;
	
	int verticeCount;
	GameObject gridObject;
	Texture2D blockTexture;
	MeshFilter mf;
	int[] meshTriangles;
	Color[] gridColors;
	Color activeSelection = Color.white;
	Color[] colorSelections = {
		Color.white,
		Color.blue,
		Color.black,
		Color.cyan,
		Color.green,
		Color.grey,
		Color.magenta,
		Color.red,
		Color.yellow,
	};
	
	void Start () {
        blockTexture = new Texture2D(1,1,TextureFormat.ARGB32,false);
        blockTexture.SetPixel(0,0,Color.white);
        blockTexture.Apply();
		
        var gridMesh = new Mesh();
        gridMesh.name = "GridMesh";
        gridObject = new GameObject("gridObject");
        gridObject.active = true;
		
   		var xSections = Mathf.FloorToInt((GridSizeWidth ) / GridSectionWidth);
   		var zSections = Mathf.FloorToInt((GridSizeHeight ) / GridSectionHeight);
        int hCount = xSections+1;
        int vCount = zSections+1;
        var numTriangles = xSections * zSections * 6;
        var numVertices = hCount * vCount;
        Vector3[] meshVertices = new Vector3[numVertices];
        meshTriangles = new int[numTriangles];
        gridColors = new Color[numVertices];

        var vertIndex = 0;
        for (var z = 0.0f; z < vCount; z++){
            for (var x = 0.0f; x < hCount; x++){
                meshVertices[vertIndex] = new Vector3(x*GridSectionWidth - (GridSizeWidth )/2f, 0.0f, z*GridSectionHeight - (GridSizeHeight )/2f);
                gridColors[vertIndex] = Color.white;
                vertIndex++;
            }
        }

        vertIndex = 0;
        for (var z= 0; z < zSections; z++){
            for (var x= 0; x < xSections; x++){
                meshTriangles[vertIndex]   = (z*hCount)+x;
                meshTriangles[vertIndex+1] = ((z+1)*hCount)+x;
                meshTriangles[vertIndex+2] = (z*hCount)+x+1;
                meshTriangles[vertIndex+3] = ((z+1)*hCount)+x;
                meshTriangles[vertIndex+4] = ((z+1)*hCount)+x+1;
                meshTriangles[vertIndex+5] = (z*hCount)+x+1;
                vertIndex += 6;
            }
        }
        gridMesh.vertices = meshVertices;
        gridMesh.colors = gridColors;
        gridMesh.triangles = meshTriangles;
        gridMesh.RecalculateNormals();
        gridMesh.RecalculateBounds();
        
        mf = gridObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gridObject.AddComponent<MeshRenderer>();
        mf.mesh = gridMesh;
        mr.material.shader = Shader.Find ("VertexColor");
        gridObject.transform.parent = Camera.mainCamera.transform;
        gridObject.transform.localPosition = new Vector3(0,0,10);
        gridObject.transform.localRotation = Quaternion.Euler(90,180,0);
        gridObject.AddComponent<MeshCollider>();
	}
	
	void Update () {
		if(Input.GetMouseButton(0)){
			var _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit _hit;
			if (Physics.Raycast(_ray,out _hit, 100.0f)){
				if (_hit.collider.gameObject == gridObject) {
				    gridColors[meshTriangles[_hit.triangleIndex * 3 + 0]]=activeSelection;
				    if(verticeCount>1)gridColors[meshTriangles[_hit.triangleIndex * 3 + 1]]=activeSelection;    
				    if(verticeCount>2)gridColors[meshTriangles[_hit.triangleIndex * 3 + 2]]=activeSelection;
					mf.mesh.colors = gridColors;
				}
			}
		}
	}
	
	void OnGUI () {
		GUI.Box(new Rect(0,0,150,150),"");
		GUI.Label(new Rect(2,2,146,20),"Vertices per brush:");
		if(GUI.Button(new Rect(10,24,20,20),"1"))
			verticeCount = 1;
		if(GUI.Button(new Rect(40,24,20,20),"2"))
			verticeCount = 2;
		if(GUI.Button(new Rect(70,24,20,20),"3"))
			verticeCount = 3;
		for(var i = 0; i<colorSelections.Length;i++){
			if(activeSelection == colorSelections[i]){
				GUI.color = Color.white;
				GUI.DrawTexture(new Rect(2+((i%4)*24),46+((i/4)*24),24,24),blockTexture);
			}
			GUI.color = colorSelections[i];
			GUI.DrawTexture(new Rect(4+((i%4)*24),48+((i/4)*24),20,20),blockTexture);
			if(new Rect(4+((i%4)*24),48+((i/4)*24),20,20).Contains(Event.current.mousePosition) 
				&& Event.current.type == EventType.MouseUp)
				activeSelection = colorSelections[i];
		}
	}
}
