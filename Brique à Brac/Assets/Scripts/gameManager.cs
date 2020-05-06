using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Windows.Kinect;
using System.Collections.Generic;

public class gameManager : MonoBehaviour //Unity ne nous laisse pas appeller la classe avec une majuscule
{                                       //Pour des raisons de clareté, nous avons donc laissé le nom gameManager au lieu de Gm ou Manager

    #region Variables
    private AudioManager audioManager;

    private TMP_Text textVies;
    private GameObject textGagne;   //On les prends en GameObject et pas en texte car on veut le mettre en Actif ou non
    private GameObject textPerdu;   //Et on se moque de changer leur texte (ou alors il aura fallut modifier l'alpha de leur couleur)

    private FileManager fileManager;
    private GameObject raquette;
    private GameObject prefabBalle;
    private Balle balle;
    private Niveau niveau;

    private int vies;
    private bool resetBalle;
    private static int nbBriques;

    private Vector3 posBaseRaquette;
    private Vector3 posBaseBalle;
    #endregion

    void Awake()
    {

        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        if (audioManager == null)
        {
            Debug.Log("pb binding audioManager");
        }

        raquette = Instantiate(Resources.Load<GameObject>("Prefabs/Raquette"));
        prefabBalle = Instantiate(Resources.Load<GameObject>("Prefabs/Balle"));

        balle = prefabBalle.GetComponent<Balle>();
        fileManager = new FileManager();

        niveau = fileManager.getNiveauChoisi();
        Instantiate(Resources.Load<GameObject>("Prefabs/" + niveau.GetNom()));

        textVies = GameObject.Find("Vies").GetComponent<TMP_Text>();
        textGagne = GameObject.Find("Gagné");
        textPerdu = GameObject.Find("Perdu");

        vies = 3;
        resetBalle = true;
        balle.IsMoving = false;
        nbBriques = niveau.NbBriques;

        posBaseRaquette = raquette.transform.position;
        posBaseBalle = balle.transform.position;

        textVies.text = "Vies : " + vies.ToString();
        textGagne.SetActive(false);
        textPerdu.SetActive(false);

    }

    void Update()
    {

        fileManager.SetPreset("Bonjour");

        if (balle == null)
        {
            return;
        }

        if (resetBalle == true)
        {
            resetBalle = false;
        }

        if (!balle.IsMoving)
        {
            balle.transform.position = new Vector3(raquette.transform.position.x, balle.transform.position.y);
        }

        if (balle.transform.position.y <= 5)
        {
            EnleverVie();
        }

        if (nbBriques <= 0)
            Gagne();
    }

    #region GameFunctions
    private void EnleverVie()
    {
        vies--;
        if (vies >= 0)
        {
            textVies.text = string.Format("Vies : {0}", vies);
            ResetBalle();
        }
        else
        {
            textVies.text = "Vies : 0";
            GameOver();
        }
    }

    private void ResetBalle()
    {
        balle.IsMoving = false;
        balle.Corps.velocity = new Vector3(0, 0, 0);
        raquette.transform.position = posBaseRaquette;
        balle.transform.position = posBaseBalle;

    }

    public static void EnleverBrique()
    {
        nbBriques--;
    }

    private void GameOver()
    {
        balle.Corps.velocity = new Vector3(0, 0, 0);
        Invoke("RelancerPartie", 3);
        if (!textPerdu.activeInHierarchy)
            textPerdu.SetActive(true);

    }

    private void Gagne()
    {
        balle.Corps.velocity = new Vector3(0, 0, 0);
        Invoke("RetourAuMenu", 3);
        if (!textGagne.activeInHierarchy)
            textGagne.SetActive(true);
    }

    private void RelancerPartie()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RetourAuMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
    #endregion
}
