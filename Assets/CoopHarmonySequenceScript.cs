using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;
using KModkit;

public class CoopHarmonySequenceScript : MonoBehaviour
{
    public KMAudio Audio;
    public KMBombInfo Bomb;

    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    public KMSelectable[] SeqBtns;
    public KMSelectable LstnBtn;
    public Light[] SeqLights;
    public AudioClip[] Sounds;
    public GameObject[] Text;
    public GameObject[] ModuleInstrumentText;
    public GameObject[] StageIndicators;
    public Material SIUnlit;
    public KMSelectable[] InsCycBtns;
    public KMRuleSeedable RuleSeedable;

    private int[][][] stages = new[]
    {
        new[]
        {
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 }
        },
        new[]
        {
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 }
        },
        new[]
        {
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 }
        },
        new[]
        {
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 },
            new[] { 0, 0, 0, 0 }
        }
    };

    private static readonly string[][][][] harmonies = new[]
    {
        new[]
        {
            new[]
            {
                new[] { "mb_d4", "mb_f4", "mb_a4", "mb_d5"},
                new[] { "mb_d4", "mb_f4", "mb_ais4", "mb_d5" },
                new[] { "mb_c4", "mb_f4", "mb_a4", "mb_c5" },
                new[] { "mb_c4", "mb_e4", "mb_g4", "mb_c5" }
            },

            new[]
            {
                new[] { "mb_cis4", "mb_f4", "mb_gis4", "mb_cis5"},
                new[] { "mb_fis4", "mb_ais4", "mb_cis5", "mb_fis5" },
                new[] { "mb_dis4", "mb_fis4", "mb_ais4", "mb_dis5" },
                new[] { "mb_c4", "mb_dis4", "mb_gis4", "mb_c5" }
            },

            new[]
            {
                new[] { "mb_a4", "mb_c5", "mb_e5", "mb_a5"},
                new[] { "mb_g4", "mb_c5", "mb_e5", "mb_g5" },
                new[] { "mb_f4", "mb_a4", "mb_c5", "mb_f5" },
                new[] { "mb_c4", "mb_e4", "mb_g4", "mb_c5" }
            },

            new[]
            {
                new[] { "mb_a4", "mb_cis5", "mb_e5", "mb_a5"},
                new[] { "mb_e4", "mb_gis4", "mb_h4", "mb_e5" },
                new[] { "mb_fis4", "mb_a4", "mb_cis5", "mb_fis5" },
                new[] { "mb_d4", "mb_fis4", "mb_a4", "mb_d5" }
            },

            new[]
            {
                new[] { "mb_d4", "mb_f4", "mb_a4", "mb_d5"},
                new[] { "mb_ais4", "mb_d5", "mb_f5", "mb_ais5" },
                new[] { "mb_f4", "mb_a4", "mb_c5", "mb_f5" },
                new[] { "mb_c4", "mb_e4", "mb_g4", "mb_c5" }
            },

            new[]
            {
                new[] { "mb_c4", "mb_e4", "mb_g4", "mb_c5" },
                new[] { "mb_e4", "mb_g4", "mb_h4", "mb_e5" },
                new[] { "mb_a4", "mb_c5", "mb_e5", "mb_a5" },
                new[] { "mb_f4", "mb_a4", "mb_c5", "mb_f5" }
            }
        },
        new[]
        {
            new[]
            {
                new[] { "p_d4", "p_f4", "p_a4", "p_d5"},
                new[] { "p_d4", "p_f4", "p_ais4", "p_d5" },
                new[] { "p_c4", "p_f4", "p_a4", "p_c5" },
                new[] { "p_c4", "p_e4", "p_g4", "p_c5" }
            },

            new[]
            {
                new[] { "p_cis4", "p_f4", "p_gis4", "p_cis5"},
                new[] { "p_fis4", "p_ais4", "p_cis5", "p_fis5" },
                new[] { "p_dis4", "p_fis4", "p_ais4", "p_dis5" },
                new[] { "p_c4", "p_dis4", "p_gis4", "p_c5" }
            },

            new[]
            {
                new[] { "p_a4", "p_c5", "p_e5", "p_a5"},
                new[] { "p_g4", "p_c5", "p_e5", "p_g5" },
                new[] { "p_f4", "p_a4", "p_c5", "p_f5" },
                new[] { "p_c4", "p_e4", "p_g4", "p_c5" }
            },

            new[]
            {
                new[] { "p_a4", "p_cis5", "p_e5", "p_a5"},
                new[] { "p_e4", "p_gis4", "p_h4", "p_e5" },
                new[] { "p_fis4", "p_a4", "p_cis5", "p_fis5" },
                new[] { "p_d4", "p_fis4", "p_a4", "p_d5" }
            },

            new[]
            {
                new[] { "p_d4", "p_f4", "p_a4", "p_d5"},
                new[] { "p_ais4", "p_d5", "p_f5", "p_ais5" },
                new[] { "p_f4", "p_a4", "p_c5", "p_f5" },
                new[] { "p_c4", "p_e4", "p_g4", "p_c5" }
            },

            new[]
            {
                new[] { "p_c4", "p_e4", "p_g4", "p_c5" },
                new[] { "p_e4", "p_g4", "p_h4", "p_e5" },
                new[] { "p_a4", "p_c5", "p_e5", "p_a5" },
                new[] { "p_f4", "p_a4", "p_c5", "p_f5" }
            }

        },
        new[]
        {
            new[]
            {
                new[] { "xy_d5", "xy_f5", "xy_a5", "xy_d6"},
                new[] { "xy_d5", "xy_f5", "xy_ais5", "xy_d6" },
                new[] { "xy_c5", "xy_f5", "xy_a5", "xy_c6" },
                new[] { "xy_c5", "xy_e5", "xy_g5", "xy_c6" }
            },

            new[]
            {
                new[] { "xy_cis5", "xy_f5", "xy_gis5", "xy_cis6"},
                new[] { "xy_fis5", "xy_ais5", "xy_cis6", "xy_fis6" },
                new[] { "xy_dis5", "xy_fis5", "xy_ais5", "xy_dis6" },
                new[] { "xy_c5", "xy_dis5", "xy_gis5", "xy_c6" }
            },

            new[]
            {
                new[] { "xy_a5", "xy_c6", "xy_e6", "xy_a6"},
                new[] { "xy_g5", "xy_c6", "xy_e6", "xy_g6" },
                new[] { "xy_f5", "xy_a5", "xy_c6", "xy_f6" },
                new[] { "xy_c5", "xy_e5", "xy_g5", "xy_c6" }
            },

            new[]
            {
                new[] { "xy_a5", "xy_cis6", "xy_e6", "xy_a6"},
                new[] { "xy_e5", "xy_gis5", "xy_h5", "xy_e6" },
                new[] { "xy_fis5", "xy_a5", "xy_cis6", "xy_fis6" },
                new[] { "xy_d5", "xy_fis5", "xy_a5", "xy_d6" }
            },

            new[]
            {
                new[] { "xy_d5", "xy_f5", "xy_a5", "xy_d6"},
                new[] { "xy_ais5", "xy_d6", "xy_f6", "xy_ais6" },
                new[] { "xy_f5", "xy_a5", "xy_c6", "xy_f6" },
                new[] { "xy_c5", "xy_e5", "xy_g5", "xy_c6" }
            },

            new[]
            {
                new[] { "xy_c5", "xy_e5", "xy_g5", "xy_c6" },
                new[] { "xy_e5", "xy_g5", "xy_h5", "xy_e6" },
                new[] { "xy_a5", "xy_c6", "xy_e6", "xy_a6" },
                new[] { "xy_f5", "xy_a5", "xy_c6", "xy_f6" }
            }
        },
        new[]
        {
            new[]
            {
                new[] { "ha_d5", "ha_f5", "ha_a5", "ha_d6"},
                new[] { "ha_d5", "ha_f5", "ha_ais5", "ha_d6" },
                new[] { "ha_c5", "ha_f5", "ha_a5", "ha_c6" },
                new[] { "ha_c5", "ha_e5", "ha_g5", "ha_c6" }
            },

            new[]
            {
                new[] { "ha_cis5", "ha_f5", "ha_gis5", "ha_cis6"},
                new[] { "ha_fis5", "ha_ais5", "ha_cis6", "ha_fis6" },
                new[] { "ha_dis5", "ha_fis5", "ha_ais5", "ha_dis6" },
                new[] { "ha_c5", "ha_dis5", "ha_gis5", "ha_c6" }
            },

            new[]
            {
                new[] { "ha_a5", "ha_c6", "ha_e6", "ha_a6"},
                new[] { "ha_g5", "ha_c6", "ha_e6", "ha_g6" },
                new[] { "ha_f5", "ha_a5", "ha_c6", "ha_f6" },
                new[] { "ha_c5", "ha_e5", "ha_g5", "ha_c6" }
            },

            new[]
            {
                new[] { "ha_a5", "ha_cis6", "ha_e6", "ha_a6"},
                new[] { "ha_e5", "ha_gis5", "ha_h5", "ha_e6" },
                new[] { "ha_fis5", "ha_a5", "ha_cis6", "ha_fis6" },
                new[] { "ha_d5", "ha_fis5", "ha_a5", "ha_d6" }
            },

            new[]
            {
                new[] { "ha_d5", "ha_f5", "ha_a5", "ha_d6"},
                new[] { "ha_ais5", "ha_d6", "ha_f6", "ha_ais6" },
                new[] { "ha_f5", "ha_a5", "ha_c6", "ha_f6" },
                new[] { "ha_c5", "ha_e5", "ha_g5", "ha_c6" }
            },

            new[]
            {
                new[] { "ha_c5", "ha_e5", "ha_g5", "ha_c6" },
                new[] { "ha_e5", "ha_g5", "ha_h5", "ha_e6" },
                new[] { "ha_a5", "ha_c6", "ha_e6", "ha_a6" },
                new[] { "ha_f5", "ha_a5", "ha_c6", "ha_f6" }
            }
        }
    };

    private bool seqFlashActive = true;
    private bool listen = false;
    private bool harmonyRunning = false;
    private bool stageCompleteActive = false;
    private bool strikeHandlerActive = false;

    private int moduleHarmony;
    private int moduleInstrument = 0;
    private int Strike = 0;
    private int currentStage = 0;
    private int correctNotes = 0;
    private int currentModuleInstrument = 0;
    private int lastModuleInstrument = 0;
    private int[][] newOrder = new[]
    {
        new [] {0, 0, 0, 0},
        new [] {0, 0, 0, 0},
        new [] {0, 0, 0, 0},
        new [] {0, 0, 0, 0}
    };
    private int[] IdentInstruments = new[] { 0, 0, 0, 0 };
    private int[] InputInstruments = new[] { 0, 0, 0, 0 };
    private bool stagePressed = false;

    private Coroutine seqFlash;

    void Awake()
    {
        moduleId = moduleIdCounter++;

        LstnBtn.OnInteract += delegate ()
        {
            if (moduleSolved)
            {
                if (harmonyRunning)
                    return false;
                StartCoroutine(Harmony());
                return false;
            }

            if (stageCompleteActive)
            {

                StartCoroutine(WaitForListen());
                return false;
            }

            LstnBtnPressed();
            return false;
        };
        LstnBtn.OnInteractEnded += delegate ()
        {
            listen = false;
            Text[0].gameObject.SetActive(false);
        };
        for (int i = 0; i < 4; i++)
        {
            SeqBtns[i].OnInteract += SeqBtnsPress(i);
        }
        for (int i = 0; i < 2; i++)
        {
            InsCycBtns[i].OnInteract += InsCycBtnsPress(i);
        }

    }

    private KMSelectable.OnInteractHandler SeqBtnsPress(int btnPressed)
    {
        return delegate ()
        {
            if (moduleSolved || strikeHandlerActive || stageCompleteActive)
                return false;

            StopCoroutine(seqFlash);
            if (seqFlashActive)
            {
                for (int i = 0; i < 4; i++)
                {
                    SeqLights[i].gameObject.SetActive(false);
                }
            }
            seqFlashActive = false;
            Match(btnPressed);
            return false;
        };
    }

    private KMSelectable.OnInteractHandler InsCycBtnsPress(int btnPressed)
    {
        return delegate ()
        {
            if (btnPressed == 0)
            {
                if (moduleInstrument == 0)
                    moduleInstrument = 3;
                else
                    moduleInstrument--;
            }
            else
            {
                if (moduleInstrument == 3)
                    moduleInstrument = 0;
                else
                    moduleInstrument++;
            }
            return false;
        };
    }

    void Start()
    {
        float scalar = transform.lossyScale.x;
        for (var i = 0; i < SeqLights.Length; i++)
            SeqLights[i].range *= scalar;

        moduleHarmony = Random.Range(0, harmonies[moduleInstrument].Length);
        seqFlash = StartCoroutine(SeqFlash());
        ScrambleStages();

        //RULESEED

        var s1cond1 = new[] { 0, 1 };
        var s1cond2 = new[] { Bomb.GetBatteryCount(), Bomb.GetIndicators().Count(), Bomb.GetPortCount(), Bomb.GetBatteryHolderCount() + Bomb.GetPortPlateCount() };
        var s2cond1 = new[] { 0, 1 };
        var s2cond2 = new[] { 15, 6, 7, 8, 9, 10, 11, 12, 13, 14, 5, 16, 17, 18, 19, 20 };
        var s3cond1 = new[] { 2, 3, 4 };
        var s3cond2 = new[] { Bomb.GetSerialNumberLetters().Count(), Bomb.GetSerialNumberNumbers().Count() };
        var s4cond = new[] { 'A', 'B', 'C', 'D', 'F', 'G', 'I', 'K', 'L', 'M', 'N', 'O', 'Q', 'R', 'S', 'T' };
        var s1inst = new[] { 0, 1, 2, 3 };
        var s2inst = new[] { 0, 1, 2, 3 };
        var s3inst = new[] { 0, 1, 2, 3 };
        var s4inst = new[] { 0, 1, 2, 3 };
        var pos1 = new[] { 0, 1, 2, 3 };
        var lr = new[] { 0, 1 };
        var pos3 = new[] { 0, 1, 2, 3 };
        var pos4 = new[] { 0, 1, 2, 3 };
        var rnd = RuleSeedable.GetRNG();
        if (rnd.Seed != 1)
        {
            rnd.ShuffleFisherYates(s1cond1);
            rnd.ShuffleFisherYates(s1cond2);
            rnd.ShuffleFisherYates(s2cond1);
            rnd.ShuffleFisherYates(s2cond2);
            rnd.ShuffleFisherYates(s3cond1);
            rnd.ShuffleFisherYates(s3cond2);
            rnd.ShuffleFisherYates(s4cond);
            rnd.ShuffleFisherYates(s1inst);
            rnd.ShuffleFisherYates(s2inst);
            rnd.ShuffleFisherYates(s3inst);
            rnd.ShuffleFisherYates(s4inst);
            rnd.ShuffleFisherYates(pos1);
            rnd.ShuffleFisherYates(lr);
            rnd.ShuffleFisherYates(pos3);
            rnd.ShuffleFisherYates(pos4);
        }
        var snDigitSum = Bomb.GetSerialNumberNumbers().Sum();

        if (s1cond1[0] == 0 ? snDigitSum >= s1cond2[0] : snDigitSum <= s1cond2[0])
        {
            IdentInstruments[0] = s1inst[0];
            InputInstruments[0] = s1inst[1];
        }
        else
        {
            IdentInstruments[0] = s1inst[3];
            InputInstruments[0] = s1inst[2];
        }

        var solvMod = Bomb.GetSolvableModuleNames().Count();

        if (s2cond1[0] == 0 ? solvMod > s2cond2[0] : solvMod < s2cond2[0])
        {
            IdentInstruments[1] = s2inst[1];
            InputInstruments[1] = s2inst[2];
        }
        else
        {
            IdentInstruments[1] = s2inst[0];
            InputInstruments[1] = s2inst[3];
        }

        if (s3cond1[0] == s3cond2[0])
        {
            IdentInstruments[2] = s3inst[2];
            InputInstruments[2] = s3inst[3];
        }
        else
        {
            IdentInstruments[2] = s3inst[1];
            InputInstruments[2] = s3inst[0];
        }
        if (Bomb.GetIndicators().Any(ch => ch.Contains(s4cond[0])))
        {
            IdentInstruments[3] = s3inst[3];
            InputInstruments[3] = s3inst[0];
        }
        else
        {
            IdentInstruments[3] = s3inst[2];
            InputInstruments[3] = s3inst[1];
        }

        newOrder[0][pos1[1]] = Array.IndexOf(stages[IdentInstruments[0]][0], pos1[3]);
        newOrder[0][pos1[3]] = Array.IndexOf(stages[IdentInstruments[0]][0], pos1[1]);
        newOrder[0][pos1[2]] = Array.IndexOf(stages[IdentInstruments[0]][0], pos1[0]);
        newOrder[0][pos1[0]] = Array.IndexOf(stages[IdentInstruments[0]][0], pos1[2]);

        newOrder[1][0] = Array.IndexOf(stages[IdentInstruments[1]][1], 3);
        newOrder[1][1] = Array.IndexOf(stages[IdentInstruments[1]][1], 2);
        newOrder[1][2] = Array.IndexOf(stages[IdentInstruments[1]][1], 1);
        newOrder[1][3] = Array.IndexOf(stages[IdentInstruments[1]][1], 0);

        var cycAmount = Array.IndexOf(stages[IdentInstruments[2]][2], pos3[0]) + 1;

        if (lr[0] == 1)
        {
            cycAmount = 4 - cycAmount;
        }

        for (int i = 0; i < 4; i++)
        {
            newOrder[2][i] = Array.IndexOf(stages[IdentInstruments[2]][2], (cycAmount + i) % 4 );
        }

        newOrder[3][0] = Array.IndexOf(stages[IdentInstruments[3]][3], 0);
        newOrder[3][1] = Array.IndexOf(stages[IdentInstruments[3]][3], 2);
        newOrder[3][2] = Array.IndexOf(stages[IdentInstruments[3]][3], 1);
        newOrder[3][3] = Array.IndexOf(stages[IdentInstruments[3]][3], 3);

        for (int i = 0; i < 4; i++)
        {
            if (i == 0) Debug.LogFormat(@"[Coop Harmony Sequence #{0}] ----------MODULE SETUP----------", moduleId, i + 1);
            Debug.LogFormat(@"[Coop Harmony Sequence #{0}] ----------Stage {1}----------", moduleId, i + 1);
            Debug.LogFormat(@"[Coop Harmony Sequence #{0}] Identify instrument/Input instrument: {2}/{3}", moduleId, i + 1, nameInstrument(i, 0), nameInstrument(i, 1));
            Debug.LogFormat(@"[Coop Harmony Sequence #{0}] Button order from lowest note to highest note: {2}, {3}, {4}, {5}", moduleId, i + 1, Array.IndexOf(stages[IdentInstruments[i]][i], 0) + 1, Array.IndexOf(stages[IdentInstruments[i]][i], 1) + 1, Array.IndexOf(stages[IdentInstruments[i]][i], 2) + 1, Array.IndexOf(stages[IdentInstruments[i]][i], 3) + 1);
            Debug.LogFormat(@"[Coop Harmony Sequence #{0}] New button order: {2}, {3}, {4}, {5}", moduleId, i + 1, newOrder[i][0] + 1, newOrder[i][1] + 1, newOrder[i][2] + 1, newOrder[i][3] + 1);
            if (i == 3) Debug.LogFormat(@"[Coop Harmony Sequence #{0}] ----------DEFUSER INPUT----------", moduleId, i + 1);
        }

        //RULESEED

    }

    void LstnBtnPressed()
    {
        listen = true;
        if (Text[3].gameObject.activeSelf)
            Text[3].gameObject.SetActive(false);
        Text[0].gameObject.SetActive(true);
    }


    void Match(int btnPressed)
    {
        if (!stagePressed) Debug.LogFormat(@"[Coop Harmony Sequence #{0}] ----------Stage {1}----------", moduleId, currentStage + 1);
        Debug.LogFormat(@"[Coop Harmony Sequence #{0}] Expected button #{1} on {3}- You pressed button #{2} on {4}", moduleId, newOrder[currentStage][correctNotes] + 1, btnPressed + 1, nameInstrument(currentStage, 1), nameInstrument(moduleInstrument, 2));
        stagePressed = true;
        if (InputInstruments[currentStage] == moduleInstrument)
        {
            if (btnPressed == newOrder[currentStage][correctNotes])
            {
                Debug.LogFormat(@"[Coop Harmony Sequence #{0}] You pressed the correct button - Well done", moduleId);
                Audio.PlaySoundAtTransform(harmonies[moduleInstrument][moduleHarmony][currentStage][stages[moduleInstrument][currentStage][btnPressed]], transform);
                SeqLights[btnPressed].gameObject.SetActive(true);
                correctNotes++;
                if (correctNotes == 4)
                {
                    correctNotes = 0;
                    StartCoroutine(StageComplete());
                }
            }
            else
            {
                StartCoroutine(StrikeHandler());
            }
        }
        else
        {
            StartCoroutine(StrikeHandler());
        }
    }

    string nameInstrument(int stage, int idin)
    {
        if (idin == 0)
        {
            if (IdentInstruments[stage] == 0) return "Music Box";
            if (IdentInstruments[stage] == 1) return "Piano";
            if (IdentInstruments[stage] == 2) return "Xylophone";
            if (IdentInstruments[stage] == 3) return "Harp";
        }
        else if (idin == 1)
        {
            if (InputInstruments[stage] == 0) return "Music Box";
            if (InputInstruments[stage] == 1) return "Piano";
            if (InputInstruments[stage] == 2) return "Xylophone";
            if (InputInstruments[stage] == 3) return "Harp";
        }
        else if (idin == 2)
        {
            if (stage == 0) return "Music Box";
            if (stage == 1) return "Piano";
            if (stage == 2) return "Xylophone";
            if (stage == 3) return "Harp";
        }

        return null;
    }

    void DisableLights()
    {
        for (int i = 0; i < 4; i++)
        {
            SeqLights[i].gameObject.SetActive(false);
        }
    }

    void ScrambleStages()
    {
        for (int d = 0; d < 4; d++)
        {
            for (int i = 0; i < 4; i++)
            {
                var sound = Enumerable.Range(0, 4).ToList();
                for (int j = 0; j < 4; j++)
                {
                    int index = Random.Range(0, sound.Count);
                    stages[d][i][j] = sound[index];
                    sound.RemoveAt(index);
                }
            }
        }
    }

    private IEnumerator StrikeHandler()
    {
        strikeHandlerActive = true;
        Text[3].gameObject.SetActive(true);
        Debug.LogFormat(@"[Coop Harmony Sequence #{0}] You pressed the wrong button - Strike", moduleId);
        correctNotes = 0;
        DisableLights();
        GetComponent<KMBombModule>().HandleStrike();
        Strike++;
        yield return new WaitForSeconds(1f);
        Text[3].gameObject.SetActive(false);
        seqFlashActive = true;
        seqFlash = StartCoroutine(SeqFlash());
        strikeHandlerActive = false;
    }

    private IEnumerator WaitForListen()
    {
        yield return new WaitUntil(() => !stageCompleteActive);
        LstnBtnPressed();
    }

    private IEnumerator StageComplete()
    {
        stageCompleteActive = true;
        yield return new WaitForSeconds(0.5f);
        DisableLights();
        yield return new WaitForSeconds(0.5f);
        Text[1].gameObject.SetActive(true);
        for (int i = 0; i < 4; i++)
        {
            SeqLights[i].gameObject.SetActive(true);
            Audio.PlaySoundAtTransform(harmonies[moduleInstrument][moduleHarmony][currentStage][i], transform);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.5f);
        DisableLights();
        StageIndicators[currentStage].GetComponent<Renderer>().material = SIUnlit;
        currentStage++;
        stagePressed = false;
        if (currentStage == 4)
        {
            Text[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            StartCoroutine(ModulePass());
            stageCompleteActive = false;
        }
        else
        {
            Text[1].gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            seqFlashActive = true;
            seqFlash = StartCoroutine(SeqFlash());
            stageCompleteActive = false;
        }
    }

    private IEnumerator ModulePass()
    {
        moduleSolved = true;
        Text[2].gameObject.SetActive(true);
        StartCoroutine(Harmony());
        yield return new WaitUntil(() => !harmonyRunning);
        GetComponent<KMBombModule>().HandlePass();
        Debug.LogFormat(@"[Coop Harmony Sequence #{0}] You passed the module - Strikes caused by this module: {1}", moduleId, Strike);
        StopAllCoroutines();
    }

    private IEnumerator Harmony()
    {
        harmonyRunning = true;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                SeqLights[j].gameObject.SetActive(true);
                Audio.PlaySoundAtTransform(harmonies[moduleInstrument][moduleHarmony][i][j], transform);
                yield return new WaitForSeconds(0.05f);
            }
            yield return new WaitForSeconds(0.5f);
            DisableLights();
            yield return new WaitForSeconds(0.5f);
        }
        harmonyRunning = false;
    }

    private IEnumerator SeqFlash()
    {
        while (seqFlashActive)
        {
            //yield return new WaitForSeconds(1f);

            for (int i = 0; i < 4; i++)
            {
                if (!seqFlashActive)
                {
                    i = 4;
                    break;
                }
                if (listen)
                    Audio.PlaySoundAtTransform(harmonies[moduleInstrument][moduleHarmony][currentStage][stages[moduleInstrument][currentStage][i]], transform);
                SeqLights[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(0.4f);
                SeqLights[i].gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        lastModuleInstrument = currentModuleInstrument;
        currentModuleInstrument = moduleInstrument;
        if (lastModuleInstrument != currentModuleInstrument)
        {
            ModuleInstrumentText[lastModuleInstrument].gameObject.SetActive(false);
            ModuleInstrumentText[currentModuleInstrument].gameObject.SetActive(true);
        }
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} start [start listening to the sequence] | !{0} stop [stop listening] | !{0} sound 1,2,3,4 [presses buttons in that order] | !{0} instrument music/xylo/piano/harp [sets the instrument] | !{0} reset [clears all inputted sounds]";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        do
        {
            yield return "trycancel";
        } while (stageCompleteActive || strikeHandlerActive);
        if (moduleSolved)
        {
            yield return "sendtochaterror The module has entered its Harmony Phase, causing this module to be solve shortly.";
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*start\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (listen)
            {
                yield return "sendtochaterror The module is already listening.";
                yield break;
            }
            yield return null;
            LstnBtn.OnInteract();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*stop\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (!listen)
            {
                yield return "sendtochaterror The module is already not listening.";
                yield break;
            }
            yield return null;
            LstnBtn.OnInteractEnded();
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            correctNotes = 0;
            DisableLights();
            yield break;
        }
        Match m;
        if ((m = Regex.Match(command, @"^\s*instrument\s+(xylo|piano|music|harp)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            yield return null;
            if (listen)
                LstnBtn.OnInteractEnded();
            var targetInstrument =
                m.Groups[1].Value.EqualsIgnoreCase("music") ? 0 :
                m.Groups[1].Value.EqualsIgnoreCase("piano") ? 1 :
                m.Groups[1].Value.EqualsIgnoreCase("xylo") ? 2 : 3;

            while (moduleInstrument != targetInstrument)
            {
                yield return new WaitForSeconds(0.1f);
                InsCycBtns[0].OnInteract();
            }
            yield break;
        }
        if ((m = Regex.Match(command, @"^\s*sound\s+([\d,;]+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
        {
            var numbers = m.Groups[1].Value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(str =>
            {
                int value;
                return int.TryParse(str, out value) ? value : (int?)null;
            }).ToArray();
            if (numbers.Length == 0 || numbers.Any(n => n == null || n.Value < 1 || n.Value > 4))
                yield break;

            yield return null;
            if (listen)
            {
                LstnBtn.OnInteractEnded();
                yield return new WaitForSeconds(.1f);
            }
            yield return numbers.Select(n => SeqBtns[n.Value - 1]).ToArray();
            yield return "solve";
            yield break;
        }
    }
}