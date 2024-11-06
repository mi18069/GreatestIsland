using System;
using System.Linq;
using UnityEngine;

public class HttpClient : MonoBehaviour
{
    private string clientEndpoint = "https://jobfair.nordeus.com/jf24-fullstack-challenge/test";

    public int[,] GetNewMatrix()
    {
        using (var client = new System.Net.Http.HttpClient())
        {
            try
            {
                var endpoint = new Uri(clientEndpoint);
                var result = client.GetAsync(endpoint).Result; // Since we have .Result we don't need to await 
                var matrixString = result.Content.ReadAsStringAsync().Result; // Expects response in string representation
                return ParseMatrixString(matrixString);
            }
            catch (Exception ex)
            {
                return new int[0,0];
            }
        }
    }

    private int[,] ParseMatrixString(string matrixString, char lineSeparator = '\n', char elementSeparator = ' ')
    {
        if (string.IsNullOrEmpty(matrixString.Trim()))
            throw new Exception();

        string[] lines = matrixString.Split(new[] { lineSeparator }, StringSplitOptions.RemoveEmptyEntries);
        int rowCount = lines.Count();
        int columnCount = 0;

        if (rowCount > 0)
        {
            columnCount = lines[0].Split(new[] { elementSeparator }, StringSplitOptions.RemoveEmptyEntries).Count();
        }

        int[,] matrix = new int[rowCount, columnCount];

        for (int i = 0; i < rowCount; i++)
        {
            string[] elements = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int j = 0; j < columnCount; j++)
            {
                if (int.TryParse(elements[j], out int height))
                    matrix[i, j] = height;
            }
        }

        return matrix;

    }

}
