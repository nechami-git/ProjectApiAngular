export class DonorModel{
    id!: number;
    identityNumber!: string;
    firstName!: string;
    lastName!: string;
    city!:string;
    email!:string;
    phone!:string;  
    donations?: DonorGiftModel[]; 
}

export class DonorFilter{
    name!: string;
    email!: string;
    giftname!: string; 
}

export class DonorGiftModel{
    id!: number;
    name!: string;
    price!: number;
}