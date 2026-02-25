
export interface PurchaseModel {
    id: number;
    giftId: number;
    giftName?: string;
    giftImage?: string;
    buyerId: number;
    buyerName?: string;
    buyerEmail?: string;
    buyerPhone?: string;
    quantity: number;
    pricePerTicket: number;
    totalPrice: number;
    purchaseDate?: Date;
}

export interface GiftPurchasersDTO {
    giftId: number;
    giftName?: string;
    purchasers: PurchaserDetailDTO[];
}

export interface PurchaserDetailDTO {
    userId: number;
    name?: string;
    email?: string;
    phone?: string;
    totalTickets: number;
    totalSpent: number;
    firstPurchaseDate?: Date;
}

export interface PurchaseFilter {
    sortBy?: PurchaseSortOption;
}

export enum PurchaseSortOption {
    Expensive= 'expensive',
    MostPurchased = 'mostpurchased',
}

