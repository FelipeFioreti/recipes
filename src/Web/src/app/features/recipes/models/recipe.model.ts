export interface Recipe {
  id: number;
  name: string;
  description: string;
  recipeTypeId: number;
  userId: number;
  createdAt: string;
  updatedAt: string;
  deletedAt?: string | null;
}

export interface SaveRecipePayload {
  name: string;
  description: string;
  recipeTypeId: number;
}

