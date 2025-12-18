using UnityEngine;
using System.Collections.Generic;

//classe padre che usa ereditarietà
public abstract class RecipeDatabase
{
    protected string moodType; // protected = accessibile nelle classi figlie
    
    // Costruttore
    public RecipeDatabase(string mood)
    {
        moodType = mood;
    }   
    
    // Metodo ASTRATTO - POLIMORFISMO
    // Ogni classe figlia DEVE implementare questo metodo
    public abstract Dictionary<string, List<string>> GetRecipes();
    
    // Metodo VIRTUALE - POLIMORFISMO
    // Può essere sovrascritto dalle classi figlie
    public virtual string GetMoodDescription()
    {
        return $"Recipe database for {moodType} mood";
    }
    
    // Metodo normale - ereditato da tutte le classi figlie
    public string GetMoodType()
    {
        return moodType;
    }
}

//  CLASSI FIGLIE 

public class HappyRecipes : RecipeDatabase
{
    // Costruttore che chiama il costruttore della classe base
    public HappyRecipes() : base("Happy") { }
    
    // OVERRIDE del metodo astratto - POLIMORFISMO
    public override Dictionary<string, List<string>> GetRecipes()
    {
        return new Dictionary<string, List<string>>()
        {
            { "Strawberry Milkshake", new List<string> { 
                "• Strawberries", 
                "• Milk" 
            }},
            { "Pastry Cream", new List<string> { 
                "• Eggs", 
                "• Lemon", 
                "• Sugar", 
                "• Corn Starch" 
            }}
        };
    }
    
    // OVERRIDE del metodo virtuale - POLIMORFISMO
    public override string GetMoodDescription()
    {
        return "😊 Happy mood recipes - Sweet and delightful!";
    }
}

public class AngryRecipes : RecipeDatabase
{
    public AngryRecipes() : base("Angry") { }
    
    public override Dictionary<string, List<string>> GetRecipes()
    {
        return new Dictionary<string, List<string>>()
        {
            { "Garlic Oil and Chilli", new List<string> { 
                "• Pasta", 
                "• Oil", 
                "• Chili Pepper", 
                "• Garlic" 
            }},
            { "Mac & Cheese", new List<string> { 
                "• Pasta", 
                "• Cheese", 
                "• Milk" 
            }}
        };
    }
    
    public override string GetMoodDescription()
    {
        return "😠 Angry mood recipes - Spicy and intense!";
    }
}

public class SadRecipes : RecipeDatabase
{
    public SadRecipes() : base("Sad") { }
    
    public override Dictionary<string, List<string>> GetRecipes()
    {
        return new Dictionary<string, List<string>>()
        {
            { "Hot Chocolate", new List<string> { 
                "• Chocolate", 
                "• Milk", 
                "• Sugar" 
            }},
            { "Mushroom Risotto", new List<string> { 
                "• Mushroom", 
                "• Rice", 
                "• Spices" 
            }}
        };
    }
    
    public override string GetMoodDescription()
    {
        return "😢 Sad mood recipes - Comforting and warm!";
    }
}

public class SickRecipes : RecipeDatabase
{
    public SickRecipes() : base("Sick") { }
    
    public override Dictionary<string, List<string>> GetRecipes()
    {
        return new Dictionary<string, List<string>>()
        {
            { "Chicken Broth", new List<string> { 
                "• Chicken", 
                "• Carrot", 
                "• Water", 
                "• Spices" 
            }},
            { "Pumpkin and Chickpea Soup", new List<string> { 
                "• Pumpkin", 
                "• Water", 
                "• Chickpea", 
                "• Spices" 
            }}
        };
    }
    
    public override string GetMoodDescription()
    {
        return "🤒 Sick mood recipes - Healing and nutritious!";
    }
}

