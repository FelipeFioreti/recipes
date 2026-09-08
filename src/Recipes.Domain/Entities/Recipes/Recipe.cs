using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Domain.Entities.Base;
using Recipes.Domain.Entities.Users;
using Recipes.Domain.Interfaces.Recipes;

namespace Recipes.Domain.Entities.Recipes;

[Table("Recipes")]
public class Recipe : BaseEntity
{
    public Recipe()
    {
    }

    public Recipe(
        string name,
        string description,
        int categoryId,
        int userId,
        IEnumerable<IRecipeIngredientData> ingredients,
        IEnumerable<IRecipeStepData> steps)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UserId = userId;
        Ingredients = ingredients.Select(ingredient => new Ingredient
        {
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            UnitId = ingredient.UnitId,
            Recipe = this
        }).ToList();
        Steps = steps.Select(step => new Step
        {
            Description = step.Description,
            Position = step.Position,
            Recipe = this
        }).ToList();
    }

    [MaxLength(255)] [Required] public string Name { get; private set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; private set; } = string.Empty;
    [ForeignKey("Category")] [Required] public int CategoryId { get; private set; }
    [ForeignKey("User")] [Required] public int UserId { get; private set; }
    public User User { get; init; } = null!;
    public Category? Category { get; init; } = null!;

    public ICollection<Ingredient> Ingredients { get; init; } = [];
    public ICollection<Step> Steps { get; init; } = [];

    public void Update(
        string name,
        string description,
        int categoryId,
        IEnumerable<IRecipeIngredientData> ingredients,
        IEnumerable<IRecipeStepData> steps)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;

        SyncIngredients(ingredients);
        SyncSteps(steps);
    }

    private void SyncIngredients(IEnumerable<IRecipeIngredientData> ingredientsData)
    {
        var existingIngredients = Ingredients.ToDictionary(ingredient => ingredient.Id);
        var processedIngredients = new HashSet<int>();

        foreach (var ingredients in ingredientsData)
        {
            if (ingredients.Id is null or 0)
            {
                Ingredients.Add(new Ingredient
                {
                    Name = ingredients.Name,
                    Quantity = ingredients.Quantity,
                    UnitId = ingredients.UnitId,
                    Recipe = this
                });

                continue;
            }

            if (!existingIngredients.TryGetValue(ingredients.Id.Value, out var ingredient))
                throw new KeyNotFoundException($"Recipe '{ingredients.Id}' not found.");

            ingredient.Update(ingredients.Name, ingredients.Quantity, ingredients.UnitId);

            processedIngredients.Add(ingredient.Id);
        }

        foreach (var ingredientToDisable in Ingredients
                     .Where(ingredient => ingredient.Id != 0 && !processedIngredients.Contains(ingredient.Id))
                     .ToList())
            ingredientToDisable.Disable();
    }

    private void SyncSteps(IEnumerable<IRecipeStepData> stepsData)
    {
        var existingSteps = Steps.ToDictionary(step => step.Id);
        var processedSteps = new HashSet<int>();

        foreach (var steps in stepsData)
        {
            if (steps.Id is null or 0)
            {
                Steps.Add(new Step
                {
                    Description = steps.Description,
                    Position = steps.Position,
                    Recipe = this
                });
                continue;
            }

            if (!existingSteps.TryGetValue(steps.Id.Value, out var step))
                throw new KeyNotFoundException($"Step '{steps.Id}' not found.");

            step.Update(steps.Description, steps.Position);
            processedSteps.Add(step.Id);
        }

        foreach (var stepsToDisable in Steps.Where(step => step.Id != 0 && !processedSteps.Contains(step.Id)).ToList())
            stepsToDisable.Disable();
    }
}