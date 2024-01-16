using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WaveFunctionCollapse : MonoBehaviour
{
    public int dimensions;
    public Tile[] tileObjects;
    public List<Cell> gridComponents;
    public Cell cellObj;

    public Tile backupTile;

    private int iteration;

    private void Awake()
    {
        gridComponents = new List<Cell>();
        InitializeGrid();
    }

    void InitializeGrid()
    {
        for(int y = 0; y < dimensions; y++)
        {
            for(int x = 0; x < dimensions; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector3(x, 0, y), Quaternion.identity);
                newCell.CreateCell(false, tileObjects);
                gridComponents.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());
    }

    IEnumerator CheckEntropy()
    {
        List<Cell> tempGrid = new List<Cell>(gridComponents);
        tempGrid.RemoveAll(c => c.collapsed);
        tempGrid.Sort((a, b) => a.tileOptions.Length - b.tileOptions.Length);
        tempGrid.RemoveAll(a => a.tileOptions.Length != tempGrid[0].tileOptions.Length);

        yield return new WaitForSeconds(0.025f);

        CollapseCell(tempGrid);
    }

    void CollapseCell(List<Cell> tempGrid)
    {
        int randIndex = UnityEngine.Random.Range(0, tempGrid.Count);

        Cell cellToCollapse = tempGrid[randIndex];

        cellToCollapse.collapsed = true;
        try
        {
            Tile selectedTile = cellToCollapse.tileOptions[UnityEngine.Random.Range(0, cellToCollapse.tileOptions.Length)];
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }
        catch
        {
            Tile selectedTile = backupTile;
            cellToCollapse.tileOptions = new Tile[] { selectedTile };
        }

        Tile foundTile = cellToCollapse.tileOptions[0];
        Instantiate(foundTile, cellToCollapse.transform.position, foundTile.transform.rotation);

        UpdateGeneration();
    }

    void UpdateGeneration()
    {
        List<Cell> newGenerationCell = new List<Cell>(gridComponents);

        for(int y = 0; y < dimensions; y++)
        {
            for(int x = 0; x < dimensions; x++)
            {
                var index = x + y * dimensions;

                if (gridComponents[index].collapsed)
                {
                    newGenerationCell[index] = gridComponents[index];
                }
                else
                {
                    List<Tile> options = new List<Tile>();
                    foreach(Tile t in tileObjects)
                    {
                        options.Add(t);
                    }

	  				int upGridIndex = x + (y - 1) * dimensions;
                    CheckDirection(IsUp(), upGridIndex);
	   
	  				int downGridIndex = x + (y + 1) * dimensions;
                    CheckDirection(IsDown(), downGridIndex);
	   
	  				int leftGridIndex = x + 1 + y * dimensions;
                    CheckDirection(IsLeft(), leftGridIndex);
	   
	  				int rightGridIndex = x - 1 + y * dimensions;
                    CheckDirection(IsRight(), rightGridIndex);                    

                    Tile[] newTileList = new Tile[options.Count];

                    for(int i = 0; i < options.Count; i++) {
                        newTileList[i] = options[i];
                    }

                    newGenerationCell[index].RecreateCell(newTileList);
                }
            }
        }

        gridComponents = newGenerationCell;
        iteration++;

        if (iteration < dimensions * dimensions)
        {
            StartCoroutine(CheckEntropy());
        }
    }

 	bool IsUp()
  	{
		return y > 0;
	}

  	bool IsDown()
  	{
		return y < dimensions - 1;
	}

  	bool IsLeft()
  	{
		return x < dimensions - 1;
	}

  	bool IsRight()
  	{
		return x > 0;
	}

 	Tile ValidTile(int index)
  	{
   		switch(direction)
		{
			case IsUp():
   				return tileObjects[index].rightNeighbours;
	   		case IsDown():
	  			return tileObjects[index].upNeighbours;
			case IsLeft():
				return tileObjects[index].downNeighbours;
	  		case IsRight():
	 			return tileObjects[index].leftNeighbours;
		}
   	}

 	void CheckDirection(bool direction, int gridIndex)
  	{
   		if(direction)
		{
			Cell newCell = gridComponents[gridIndex];
			List<Tile> validOptions = new List<Tile>();

			foreach(Tile possibleOptions in newCell.tileOptions)
			{
				var validOption = Array.FindIndex(tileObjects, obj => obj == possibleOptions);				
				var valid = ValidTile(validOption);

				validOptions = validOptions.Concat(valid).ToList();
			}

			CheckValidity(options, validOptions);
		}
   	}

    void CheckValidity(List<Tile> optionList, List<Tile> validOption)
    {
        for(int x = optionList.Count - 1; x >=0; x--)
        {
            var element = optionList[x];
            if (!validOption.Contains(element))
            {
                optionList.RemoveAt(x);
            }
        }
    }
}
