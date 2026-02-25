export interface AddToCart {
  giftId: number;
  quantity: number;
}

export interface UpdateCartItem {
  giftId: number;
  quantity: number;
}

export interface CartItem {
  id: number;
  giftId: number;
  giftName: string;
  giftImage: string;
  ticketPrice: number;
  quantity: number;
  totalPrice: number;
}