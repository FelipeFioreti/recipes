using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.BaseEntities;
using Recipes.Api.Domain.Entities.Users;
using Recipes.Api.Domain.Utils;

namespace Recipes.Api.Domain.Entities.Recipes;

[Table("Recipes")]
public class Recipe : BaseEntity
{
    public Recipe()
    {
    }

    public Recipe(CreateRecipeRequest request, int userId)
    {
        Name = request.Name;
        Description = request.Description;
        CategoryId = request.CategoryId;
        UserId = userId;
        Ingredients = request.Ingredients.Select(ingredient => new Ingredient
        {
            Name = ingredient.Name,
            Quantity = ingredient.Quantity,
            UnitId = ingredient.UnitId,
            Recipe = this
        }).ToList();
        Steps = request.Steps.Select(step => new Step
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

    public void Update(UpdateRecipeRequest request)
    {
        Name = request.Name;
        Description = request.Description;
        CategoryId = request.CategoryId;

        CollectionSync.Sync(
            Ingredients,
            request.Ingredients,
            ingredient => ingredient.Id,
            recipeIngredientRequest => recipeIngredientRequest.Id.GetValueOrDefault(),
            recipeIngredientRequest => recipeIngredientRequest.Id is null or 0,
            recipeIngredientRequest => new Ingredient
            {
                Name = recipeIngredientRequest.Name,
                Quantity = recipeIngredientRequest.Quantity,
                UnitId = recipeIngredientRequest.UnitId
            },
            (ingredient, recipeIngredientRequest) => { ingredient.Update(recipeIngredientRequest); });

        CollectionSync.Sync(
            Steps,
            request.Steps,
            step => step.Id,
            stepRequest => stepRequest.Id.GetValueOrDefault(),
            stepRequest => stepRequest.Id is null or 0,
            stepRequest => new Step
            {
                Description = stepRequest.Description,
                Position = stepRequest.Position
            },
            (step, updateStepRequest) => { step.Update(updateStepRequest); });
    }

    public RecipeResponse ToResponse()
    {
        return new RecipeResponse(this);
    }
}