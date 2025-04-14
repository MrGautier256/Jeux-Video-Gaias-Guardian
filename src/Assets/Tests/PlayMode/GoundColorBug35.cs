using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.TestTools;
using UnityEngine;
using NUnit.Framework;
using System.IO;

//https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/issues/35

public class GoundColorBug35
{

    // Référence de la couleur attendue pour la zone de test 
    private Color expectedColor = new Color(46f/255, 45f/255, 41f/255); 
   // private Color expectedColor = new Color(170f / 255, 186f / 255, 205f / 255); Valeur quand il y a le bug

    // Zone de test : en bas à gauche, avec une taille de 10x10 pixels (100 pixels au total)
    private Rect testArea = new Rect(0, 0, 10, 10);

    // Tolérance pour la comparaison de couleur
    private float tolerance = 0.1f; // Tolérance de 10% sur chaque composant de couleur


    private void SaveScreenshot(Texture2D screenshot)
    {
        var screenshotPath = @"d:\temp\";
        var screenshotFileName = "unity001.png";
        // Convertir la texture en tableau d'octets (PNG)
        byte[] screenshotBytes = screenshot.EncodeToPNG();

        // S'assurer que le dossier de destination existe
        if (!Directory.Exists(screenshotPath))
        {
            Directory.CreateDirectory(screenshotPath);
        }

        // Chemin complet du fichier
        string fullPath = Path.Combine(screenshotPath, screenshotFileName);

        // Sauvegarder le fichier
        File.WriteAllBytes(fullPath, screenshotBytes);

        Debug.Log($"Screenshot saved at: {fullPath}");
    }

    [UnityTest]
    public IEnumerator TestAverageColorInBottomLeft()
    {
        // Attendre un frame pour s'assurer que tout est chargé et que l'écran est prêt à être capturé
        yield return null;

        // Capture d'écran en tant que texture
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        SaveScreenshot(screenshot);
        // Extraire la zone d'intérêt (en bas à gauche)
        Color[] pixels = screenshot.GetPixels(
            (int)testArea.x,
            (int)testArea.y,
            (int)testArea.width,
            (int)testArea.height
        );

        // Calculer la moyenne des couleurs dans la zone
        Color averageColor = CalculateAverageColor(pixels);

        // Vérifier si la couleur moyenne est proche de la couleur attendue
        Assert.IsTrue(IsColorClose(averageColor, expectedColor),
                      $"La couleur moyenne {averageColor} ne correspond pas à la couleur attendue {expectedColor}.");

        // Libérer la mémoire de la texture capturée
        UnityEngine.Object.Destroy(screenshot);
    }

    // Méthode pour calculer la couleur moyenne d'un tableau de pixels
    private Color CalculateAverageColor(Color[] pixels)
    {
        float r = 0, g = 0, b = 0;

        foreach (var pixel in pixels)
        {
            r += pixel.r;
            g += pixel.g;
            b += pixel.b;
        }

        int pixelCount = pixels.Length;
        return new Color(r / pixelCount, g / pixelCount, b / pixelCount);
    }

    // Méthode pour vérifier si deux couleurs sont proches (en tenant compte de la tolérance)
    private bool IsColorClose(Color color1, Color color2)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }
}

