export interface RecipeType {
  id: number;
  name: string;
  createdAt: string;
  updatedAt: string;
  deletedAt?: string | null;
}

export interface SaveRecipeTypePayload {
  name: string;
}

