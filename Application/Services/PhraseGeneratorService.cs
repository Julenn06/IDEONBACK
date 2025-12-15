namespace IdeonBack.Application.Services;

/// <summary>
/// Servicio para generar frases aleatorias para PhotoClash
/// </summary>
public class PhraseGeneratorService
{
    private readonly Dictionary<string, List<string>> _phrasesByLanguage = new()
    {
        ["es"] = new List<string>
        {
            "Algo que te haga reír",
            "Tu lugar favorito de la casa",
            "Lo más viejo que tienes cerca",
            "Algo azul",
            "Tu comida favorita",
            "Un objeto redondo",
            "Algo que uses todos los días",
            "Lo más raro que encuentres",
            "Tu zapato izquierdo",
            "Un selfie gracioso",
            "Algo verde",
            "Tu bebida favorita",
            "Un objeto rectangular",
            "Algo brillante",
            "Lo más pequeño que encuentres",
            "Tu posesión más valiosa",
            "Algo rojo",
            "Un libro o revista",
            "Algo suave",
            "Tu mejor imitación de un animal"
        },
        ["en"] = new List<string>
        {
            "Something that makes you laugh",
            "Your favorite place at home",
            "The oldest thing near you",
            "Something blue",
            "Your favorite food",
            "A round object",
            "Something you use every day",
            "The weirdest thing you can find",
            "Your left shoe",
            "A funny selfie",
            "Something green",
            "Your favorite drink",
            "A rectangular object",
            "Something shiny",
            "The smallest thing you can find",
            "Your most valuable possession",
            "Something red",
            "A book or magazine",
            "Something soft",
            "Your best animal impression"
        }
    };

    private readonly Dictionary<string, List<string>> _nsfwPhrasesByLanguage = new()
    {
        ["es"] = new List<string>
        {
            "Tu ropa interior más loca",
            "Algo que no deberías tener",
            "Tu peor foto de perfil",
            "Algo embarazoso",
            "Tu postura de yoga más ridícula"
        },
        ["en"] = new List<string>
        {
            "Your craziest underwear",
            "Something you shouldn't have",
            "Your worst profile picture",
            "Something embarrassing",
            "Your most ridiculous yoga pose"
        }
    };

    public string GenerateRandomPhrase(string language = "es", bool nsfwAllowed = false)
    {
        var random = new Random();
        var availablePhrases = new List<string>();

        if (_phrasesByLanguage.ContainsKey(language))
        {
            availablePhrases.AddRange(_phrasesByLanguage[language]);
        }
        else
        {
            availablePhrases.AddRange(_phrasesByLanguage["es"]);
        }

        if (nsfwAllowed && _nsfwPhrasesByLanguage.ContainsKey(language))
        {
            availablePhrases.AddRange(_nsfwPhrasesByLanguage[language]);
        }

        return availablePhrases[random.Next(availablePhrases.Count)];
    }

    public List<string> GenerateUniquePhrases(int count, string language = "es", bool nsfwAllowed = false)
    {
        var random = new Random();
        var availablePhrases = new List<string>();

        if (_phrasesByLanguage.ContainsKey(language))
        {
            availablePhrases.AddRange(_phrasesByLanguage[language]);
        }
        else
        {
            availablePhrases.AddRange(_phrasesByLanguage["es"]);
        }

        if (nsfwAllowed && _nsfwPhrasesByLanguage.ContainsKey(language))
        {
            availablePhrases.AddRange(_nsfwPhrasesByLanguage[language]);
        }

        return availablePhrases
            .OrderBy(x => random.Next())
            .Take(count)
            .ToList();
    }
}
