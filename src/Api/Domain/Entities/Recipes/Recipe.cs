using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Users;

namespace Recipes.Api.Domain.Entities.Recipes;

[Table("Recipes")]
public class Recipe : BaseEntity
{
    public Recipe()
    {
    }

    public Recipe(string name, string description, int categoryId, int userId)
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UserId = userId;
    }

    [MaxLength(255)] [Required] public string Name { get; private set; } = string.Empty;
    [MaxLength(2000)] public string Description { get; private set; } = string.Empty;
    [ForeignKey("Category")] [Required] public int CategoryId { get; private set; }
    [ForeignKey("User")] [Required] public int UserId { get; private set; }
    public User User { get; set; } = null!;
    public Category? Category { get; set; } = null!;

    public IList<Ingredient> Ingredients { get; set; } = [];
    public IList<Step> Steps { get; set; } = [];

    public void Update(UpdateRecipeRequest request)
    {
        Name = request.Name;
        Description = request.Description;
        CategoryId = request.CategoryId;
        Ingredients = MapIngredients(request.Ingredients);
        Steps = MapSteps(request.Steps);
    }

    public void AddChildren(CreateRecipeRequest request)
    {
        foreach (var ingredient in request.Ingredients)
        {
            Ingredients.Add(new Ingredient(ingredient.Name, ingredient.Quantity, 0, ingredient.UnitId)
            {
                Recipe = this
            });
        }

        foreach (var step in request.Steps)
        {
            Steps.Add(new Step(0, step.Position, step.Description)
            {
                Recipe = this
            });
        }
    }

    private List<Ingredient> MapIngredients(IEnumerable<RecipeIngredientRequest> requests)
    {
        var existingIngredients = Ingredients.ToDictionary(ingredient => ingredient.Id);
        var mappedIngredients = new List<Ingredient>();

        foreach (var request in requests)
        {
            if (request.Id.HasValue && existingIngredients.TryGetValue(request.Id.Value, out var ingredient))
            {
                ingredient.Name = request.Name;
                ingredient.Quantity = request.Quantity;
                ingredient.UnitId = request.UnitId;
                mappedIngredients.Add(ingredient);
            }
            else
            {
                mappedIngredients.Add(new Ingredient(request.Id, request.Name, request.Quantity, Id, request.UnitId));
            }
        }

        return mappedIngredients;
    }

    private List<Step> MapSteps(IEnumerable<RecipeStepRequest> requests)
    {
        var existingSteps = Steps.ToDictionary(step => step.Id);
        var mappedSteps = new List<Step>();

        foreach (var request in requests)
        {
            if (request.Id.HasValue && existingSteps.TryGetValue(request.Id.Value, out var step))
            {
                step.Description = request.Description;
                step.Position = request.Position;
                mappedSteps.Add(step);
            }
            else
            {
                mappedSteps.Add(new Step(request.Id, Id, request.Position, request.Description));
            }
        }

        return mappedSteps;
    }
}
