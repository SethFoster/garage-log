export interface Car {
  id: number;
  make: string;
  model: string;
  nickname?: string | null;
  year?: string | null;
}

export interface ModItem {
  id: number;
  name: string;
  category: string;
  cost: number;
  status: string;
  notes?: string | null;
  link?: string | null;
  createdDate: string;
  completedDate?: string | null;
  carId?: number | null;
  car?: Car | null;
  photoPath?: string | null;
  receiptPath?: string | null;
}
