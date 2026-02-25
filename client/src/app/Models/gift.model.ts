export class GiftModel{
    id!:number;
    name!: string;
    description!: string;
    image!: string;
    price!: number;
    ticketPrice!: number;

    categoryId!:number;
    categoryName?:string;

    donorId!:number;
    donorName?:string;

    participantsCount!:number;
    winnerName?:string;
}
export class GiftFilter{
    name?:string;
    CategoryName?:string;
    TicketPrice?:number;
}
export class GiftFilterAdmin{
    name?:string;
    donorName?:string;
    purchaseCount?:number;
}